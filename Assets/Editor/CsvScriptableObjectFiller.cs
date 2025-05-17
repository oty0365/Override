using UnityEngine;
using UnityEditor;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

public class CsvScriptableObjectFiller : EditorWindow
{
    private string csvPath = "";
    private ScriptableObject targetSO;
    private List<string> csvHeaders = new List<string>();
    private List<string[]> csvRows = new List<string[]>();
    private Dictionary<string, string> mapping = new Dictionary<string, string>();

    [MenuItem("Tools/CSV ↔ ScriptableObject 매핑툴")]
    public static void ShowWindow()
    {
        GetWindow<CsvScriptableObjectFiller>("CSV Mapper");
    }

    void OnGUI()
    {
        EditorGUILayout.Space();

        if (GUILayout.Button("CSV 파일 선택"))
        {
            string path = EditorUtility.OpenFilePanel("CSV 선택", "", "csv");
            if (!string.IsNullOrEmpty(path))
            {
                csvPath = path;
                LoadCSV();
            }
        }

        if (!string.IsNullOrEmpty(csvPath))
            EditorGUILayout.LabelField("CSV 경로:", csvPath);

        targetSO = (ScriptableObject)EditorGUILayout.ObjectField("ScriptableObject", targetSO, typeof(ScriptableObject), false);

        if (targetSO != null && csvHeaders.Count > 0)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("CSV → ScriptableObject 매핑:", EditorStyles.boldLabel);

            var fields = targetSO.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            string[] fieldNames = new string[fields.Length];
            for (int i = 0; i < fields.Length; i++)
                fieldNames[i] = fields[i].Name;

            foreach (string csvField in csvHeaders)
            {
                if (!mapping.ContainsKey(csvField))
                    mapping[csvField] = "";

                int selected = System.Array.IndexOf(fieldNames, mapping[csvField]);
                int newSelected = EditorGUILayout.Popup(csvField, selected, fieldNames);
                if (newSelected >= 0 && newSelected < fieldNames.Length)
                    mapping[csvField] = fieldNames[newSelected];
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("매핑 적용"))
            {
                ApplyCSVToSO();
            }
        }
    }

    void LoadCSV()
    {
        csvHeaders.Clear();
        csvRows.Clear();

        string[] lines = File.ReadAllLines(csvPath);
        if (lines.Length == 0) return;

        csvHeaders.AddRange(lines[0].Split(','));

        for (int i = 1; i < lines.Length; i++)
        {
            csvRows.Add(lines[i].Split(','));
        }
    }

    void ApplyCSVToSO()
    {
        if (csvRows.Count == 0 || targetSO == null) return;

        var type = targetSO.GetType();
        string[] firstRow = csvRows[0];

        foreach (var pair in mapping)
        {
            string csvField = pair.Key;
            string soField = pair.Value;

            if (string.IsNullOrEmpty(soField)) continue;

            int colIndex = csvHeaders.IndexOf(csvField);
            if (colIndex < 0 || colIndex >= firstRow.Length) continue;

            FieldInfo field = type.GetField(soField, BindingFlags.Public | BindingFlags.Instance);
            if (field == null) continue;

            string rawValue = firstRow[colIndex].Trim();
            object parsedValue = ConvertValue(rawValue, field.FieldType);
            if (parsedValue != null)
            {
                field.SetValue(targetSO, parsedValue);
            }
        }

        EditorUtility.SetDirty(targetSO);
        AssetDatabase.SaveAssets();
        Debug.Log("ScriptableObject 값 적용 완료!");
    }

    static object ConvertValue(string value, System.Type type)
    {
        try
        {
            if (type == typeof(int)) return int.Parse(value);
            if (type == typeof(float)) return float.Parse(value);
            if (type == typeof(bool)) return bool.Parse(value);
            if (type == typeof(string)) return value;
            if (type.IsEnum) return System.Enum.Parse(type, value);
        }
        catch
        {
            Debug.LogWarning($"값 변환 실패: {value} → {type.Name}");
        }
        return null;
    }
}