using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(CameraBoundArea))]
public class CameraBoundAreaEditor : Editor
{
    private const float DefaultAspect = 16f / 9f;
    private bool keepAspectRatio = true; 

    private void OnSceneGUI()
    {
        var area = (CameraBoundArea)target;
        var t = area.transform;

        Vector3 center = t.position;
        Vector2 size = area.size;
        Vector2 half = size * 0.5f;

        float aspectRatio = DefaultAspect;

        if (SceneView.lastActiveSceneView != null && SceneView.lastActiveSceneView.camera != null)
        {
            var cam = SceneView.lastActiveSceneView.camera;
            aspectRatio = cam.aspect;
        }

        Handles.color = Color.green;

        Vector3 rightHandle = center + new Vector3(half.x, 0, 0);
        Vector3 leftHandle = center + new Vector3(-half.x, 0, 0);

        EditorGUI.BeginChangeCheck();

        rightHandle = Handles.Slider(rightHandle, Vector3.right);
        leftHandle = Handles.Slider(leftHandle, Vector3.left);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(area, "Resize Camera Bound");

            float width = (rightHandle.x - leftHandle.x);
            float height = keepAspectRatio ? width / aspectRatio : area.size.y;

            area.size = new Vector2(width, height);

            t.position = new Vector3(
                (rightHandle.x + leftHandle.x) / 2f,
                t.position.y,
                t.position.z
            );
        }

        Vector2 finalHalf = area.size * 0.5f;
        Vector3[] rectCorners = new Vector3[]
        {
            center + new Vector3(-finalHalf.x, -finalHalf.y),
            center + new Vector3(finalHalf.x, -finalHalf.y),
            center + new Vector3(finalHalf.x, finalHalf.y),
            center + new Vector3(-finalHalf.x, finalHalf.y),
        };
        for (int i = 0; i < 4; i++)
            Handles.DrawLine(rectCorners[i], rectCorners[(i + 1) % 4]);
    }
}
#endif
