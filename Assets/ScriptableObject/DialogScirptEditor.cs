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
        EditorGUILayout.LabelField("Dialog Script 설정", headerStyle);
        EditorGUILayout.Space(10);

        EditorGUILayout.PropertyField(talker, new GUIContent("대화자 이름"));
        EditorGUILayout.PropertyField(hasSelection, new GUIContent("선택지 여부"));

        if (!hasSelection.boolValue)
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("대화자 초상", EditorStyles.boldLabel);

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
                EditorGUILayout.HelpBox("스프라이트가 선택되지 않았습니다.", MessageType.Warning);
            }

            EditorGUILayout.PropertyField(dialogue, new GUIContent("대사 내용"));
        }
        else
        {
            EditorGUILayout.HelpBox("선택지 모드에서는 초상과 대사가 비활성화됩니다.", MessageType.Info);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
