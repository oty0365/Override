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
        EditorGUILayout.LabelField("��������", headerStyle);
        EditorGUILayout.Space(10);

        EditorGUILayout.PropertyField(augmentType, new GUIContent("���� Ÿ��"));
        EditorGUILayout.PropertyField(augmentName, new GUIContent("�����̸�"));
        EditorGUILayout.Space(5);
        EditorGUILayout.PropertyField(augmentSprite, new GUIContent("�����̹���"));
        EditorGUILayout.Space(10);
        EditorGUILayout.PropertyField(augmentDesc, new GUIContent("��������"));
        EditorGUILayout.Space(10);
        EditorGUILayout.PropertyField(effect, new GUIContent("�̺�Ʈ"));
        serializedObject.ApplyModifiedProperties();
    }
}
#endif