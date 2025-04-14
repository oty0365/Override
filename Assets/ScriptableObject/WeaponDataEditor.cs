using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WeaponData))]
public class WeaponDataEditor : Editor
{
    private SerializedProperty weaponCode;
    private SerializedProperty weaponName;
    private SerializedProperty weaponSprite;
    private SerializedProperty weaponDesc;
    private SerializedProperty weaponRange;
    private SerializedProperty weaponStartDamage;
    private SerializedProperty weaponAttackSpeed;
    private SerializedProperty weaponColor;
    private SerializedProperty weaponPrefab;

    private void OnEnable()
    {
        weaponCode = serializedObject.FindProperty("weaponCode");
        weaponName = serializedObject.FindProperty("weaponName");
        weaponSprite = serializedObject.FindProperty("weaponSprite");
        weaponDesc = serializedObject.FindProperty("weaponDesc");
        weaponRange = serializedObject.FindProperty("weaponRange");
        weaponStartDamage = serializedObject.FindProperty("weaponStartDamage");
        weaponAttackSpeed = serializedObject.FindProperty("weaponAttackSpeed");
        weaponColor = serializedObject.FindProperty("weaponColor");
        weaponPrefab = serializedObject.FindProperty("weaponPrefab");
        
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUILayout.Space(10);
        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel);
        headerStyle.fontSize = 14;
        headerStyle.normal.textColor = new Color(0.2f, 0.6f, 1f);
        GUILayout.Label("���� ����", headerStyle);
        GUILayout.Space(5);

        EditorGUILayout.PropertyField(weaponCode, new GUIContent("���� �ڵ�"));
        EditorGUILayout.PropertyField(weaponName, new GUIContent("���� �̸�"));

        GUILayout.Space(10);
        GUILayout.Label("���� ��������Ʈ", headerStyle);
        GUILayout.Space(5);
        EditorGUILayout.PropertyField(weaponSprite, new GUIContent("��������Ʈ"));

        if (weaponSprite.objectReferenceValue != null)
        {
            GUILayout.Label(((Sprite)weaponSprite.objectReferenceValue).texture, GUILayout.Height(100));
        }
        GUILayout.Space(10);
        GUILayout.Label("���� ������", headerStyle);
        GUILayout.Space(5);
        EditorGUILayout.PropertyField(weaponPrefab, new GUIContent("������"));

        GUILayout.Space(10);
        GUILayout.Label("���� ����", headerStyle);
        GUILayout.Space(5);
        EditorGUILayout.PropertyField(weaponDesc, new GUIContent("����"));

        GUILayout.Space(10);
        GUILayout.Label("�� �� ����", headerStyle);
        GUILayout.Space(5);
        EditorGUILayout.Slider(weaponRange, 0f, 20f, new GUIContent("�⺻ ��Ÿ�"));
        GUILayout.Space(5);
        EditorGUILayout.Slider(weaponAttackSpeed, 0f, 2.5f, new GUIContent("���� ������"));
        GUILayout.Space(5);
        EditorGUILayout.PropertyField((weaponStartDamage), new GUIContent("���ʰ��ݷ�"));
        GUILayout.Space(5);
        EditorGUILayout.PropertyField((weaponColor), new GUIContent("���� �����ӻ�"));
        serializedObject.ApplyModifiedProperties();
    }
}

