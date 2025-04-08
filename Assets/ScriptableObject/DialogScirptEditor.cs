using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DialogScript))]
public class DialogScriptEditor : Editor
{
    SerializedProperty talker;
    SerializedProperty hasSelection;
    SerializedProperty talkersFace;
    SerializedProperty dialogue;

    private void OnEnable()
    {
        talker = serializedObject.FindProperty("talker");
        hasSelection = serializedObject.FindProperty("hasSelection");
        talkersFace = serializedObject.FindProperty("talkersFace");
        dialogue = serializedObject.FindProperty("dialogue");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel);
        headerStyle.fontSize = 14;
        headerStyle.normal.textColor = new Color(0.25f, 0.6f, 1f);

        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Dialog Script ����", headerStyle);
        EditorGUILayout.Space(10);

        EditorGUILayout.PropertyField(talker, new GUIContent("��ȭ�� �̸�"));
        EditorGUILayout.PropertyField(hasSelection, new GUIContent("������ ����"));

        if (!hasSelection.boolValue)
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("��ȭ�� �ʻ�", EditorStyles.boldLabel);

            talkersFace.objectReferenceValue = EditorGUILayout.ObjectField(
                talkersFace.objectReferenceValue,
                typeof(Sprite),
                false,
                GUILayout.Width(128),
                GUILayout.Height(128)
            );

            Sprite sprite = talkersFace.objectReferenceValue as Sprite;
            if (sprite != null)
            {
                Rect previewRect = GUILayoutUtility.GetRect(128, 128, GUILayout.ExpandWidth(false));
            }
            else
            {
                EditorGUILayout.HelpBox("��������Ʈ�� ���õ��� �ʾҽ��ϴ�.", MessageType.Warning);
            }

            EditorGUILayout.PropertyField(dialogue, new GUIContent("��� ����"));
        }
        else
        {
            EditorGUILayout.HelpBox("������ ��忡���� �ʻ�� ��簡 ��Ȱ��ȭ�˴ϴ�.", MessageType.Info);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
