using UnityEngine;

public class CameraBoundArea : MonoBehaviour
{
    public Vector2 size = new Vector2(10, 10);

    public Rect GetWorldRect()
    {
        Vector2 pos = (Vector2)transform.position;
        Vector2 halfSize = size * 0.5f;
        return new Rect(pos - halfSize, size);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green; 
        Rect rect = GetWorldRect(); 
        Gizmos.DrawWireCube(rect.center, rect.size);
    }
}
