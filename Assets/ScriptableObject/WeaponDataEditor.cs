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
        GUILayout.Label("무기 정보", headerStyle);
        GUILayout.Space(5);

        EditorGUILayout.PropertyField(weaponCode, new GUIContent("무기 코드"));
        EditorGUILayout.PropertyField(weaponName, new GUIContent("무기 이름"));

        GUILayout.Space(10);
        GUILayout.Label("무기 스프라이트", headerStyle);
        GUILayout.Space(5);
        EditorGUILayout.PropertyField(weaponSprite, new GUIContent("스프라이트"));

        if (weaponSprite.objectReferenceValue != null)
        {
            GUILayout.Label(((Sprite)weaponSprite.objectReferenceValue).texture, GUILayout.Height(100));
        }
        GUILayout.Space(10);
        GUILayout.Label("무기 프리팹", headerStyle);
        GUILayout.Space(5);
        EditorGUILayout.PropertyField(weaponPrefab, new GUIContent("프리팹"));

        GUILayout.Space(10);
        GUILayout.Label("무기 설명", headerStyle);
        GUILayout.Space(5);
        EditorGUILayout.PropertyField(weaponDesc, new GUIContent("설명"));

        GUILayout.Space(10);
        GUILayout.Label("그 외 정보", headerStyle);
        GUILayout.Space(5);
        EditorGUILayout.Slider(weaponRange, 0f, 20f, new GUIContent("기본 사거리"));
        GUILayout.Space(5);
        EditorGUILayout.Slider(weaponAttackSpeed, 0f, 2.5f, new GUIContent("공속 딜레이"));
        GUILayout.Space(5);
        EditorGUILayout.PropertyField((weaponStartDamage), new GUIContent("기초공격력"));
        GUILayout.Space(5);
        EditorGUILayout.PropertyField((weaponColor), new GUIContent("무기 프레임색"));
        serializedObject.ApplyModifiedProperties();
    }
}

