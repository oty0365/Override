using UnityEditor;
using UnityEngine;

/*public class AugmentDatasEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("프로젝트에서 증강오브젝트 찾기"))
        {
            FindAndAssignScriptableObjects();
        }
    }

    private void FindAndAssignScriptableObjects()
    {
        AugmentDatas array = (AugmentDatas)target;

        string[] guids = AssetDatabase.FindAssets("t:AugmentData");
        AugmentData[] foundObjects = new AugmentData[guids.Length];

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            foundObjects[i] = AssetDatabase.LoadAssetAtPath<AugmentData>(path);
        }

        array.augments = foundObjects;

        EditorUtility.SetDirty(array);
        Debug.Log($"{foundObjects.Length}개의 ScriptableObject를 찾았습니다.");
    }
}
*/