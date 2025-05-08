using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(AugmentData))]
public class AugmentDataEditor : Editor
{
    SerializedProperty augmentSprite;
    SerializedProperty augmentName;
    SerializedProperty augmentType;
    SerializedProperty augmentDesc;
    SerializedProperty effect;

    private void OnEnable()
    {
        augmentSprite = serializedObject.FindProperty("augmentSprite");
        augmentName = serializedObject.FindProperty("augmentName");
        augmentType = serializedObject.FindProperty("augmentType");
        augmentDesc = serializedObject.FindProperty("augmentDesc");
        effect = serializedObject.FindProperty("effect");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel);
        headerStyle.fontSize = 14;
        headerStyle.normal.textColor = new Color(0.25f, 0.6f, 1f);

        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("증강설정", headerStyle);
        EditorGUILayout.Space(10);

        EditorGUILayout.PropertyField(augmentType, new GUIContent("증강 타입"));
        EditorGUILayout.PropertyField(augmentName, new GUIContent("증강이름"));
        EditorGUILayout.Space(5);
        EditorGUILayout.PropertyField(augmentSprite, new GUIContent("증강이미지"));
        EditorGUILayout.Space(10);
        EditorGUILayout.PropertyField(augmentDesc, new GUIContent("증강설명"));
        EditorGUILayout.Space(10);
        EditorGUILayout.PropertyField(effect, new GUIContent("이벤트"));
        serializedObject.ApplyModifiedProperties();
    }
}
#endif