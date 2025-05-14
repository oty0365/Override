using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(OverrideablesArray))]
public class OverrideablesArrayEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("프로젝트에서 오버라이드 오브젝트 찾기"))
        {
            FindAndAssignScriptableObjects();
        }
    }

    private void FindAndAssignScriptableObjects()
    {
        OverrideablesArray array = (OverrideablesArray)target;

        string[] guids = AssetDatabase.FindAssets("t:OverrideablesData");
        OverrideablesData[] foundObjects = new OverrideablesData[guids.Length];

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            foundObjects[i] = AssetDatabase.LoadAssetAtPath<OverrideablesData>(path);
        }

        array.overrideablesList = foundObjects;

        EditorUtility.SetDirty(array);
        Debug.Log($"{foundObjects.Length}개의 ScriptableObject를 찾았습니다.");
    }
}
