using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CameraBoundArea))]
public class CameraBoundAreaEditor : Editor
{
    private void OnSceneGUI()
    {
        var area = (CameraBoundArea)target;
        var t = area.transform;

        Vector3 center = t.position;
        Vector2 size = area.size;
        Vector2 half = size * 0.5f;

        Handles.color = Color.green;

        Vector3 rightHandle = center + new Vector3(half.x, 0, 0);
        Vector3 leftHandle = center + new Vector3(-half.x, 0, 0);
        Vector3 topHandle = center + new Vector3(0, half.y, 0);
        Vector3 bottomHandle = center + new Vector3(0, -half.y, 0);

        EditorGUI.BeginChangeCheck();

        rightHandle = Handles.Slider(rightHandle, Vector3.right);
        leftHandle = Handles.Slider(leftHandle, Vector3.left);
        topHandle = Handles.Slider(topHandle, Vector3.up);
        bottomHandle = Handles.Slider(bottomHandle, Vector3.down);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(area, "Resize Camera Bound");

            float width = (rightHandle.x - leftHandle.x);
            float height = (topHandle.y - bottomHandle.y);
            area.size = new Vector2(width, height);

            t.position = new Vector3(
                (rightHandle.x + leftHandle.x) / 2f,
                (topHandle.y + bottomHandle.y) / 2f,
                t.position.z
            );
        }

        Vector3[] rectCorners = new Vector3[]
        {
            center + new Vector3(-half.x, -half.y),
            center + new Vector3(half.x, -half.y),
            center + new Vector3(half.x, half.y),
            center + new Vector3(-half.x, half.y),
        };
        for (int i = 0; i < 4; i++)
            Handles.DrawLine(rectCorners[i], rectCorners[(i + 1) % 4]);
    }
}
