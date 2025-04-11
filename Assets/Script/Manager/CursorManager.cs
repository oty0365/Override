using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D baiscCursor;
    void Start()
    {
        Cursor.SetCursor(baiscCursor, Vector2.zero, CursorMode.ForceSoftware);
    }
}
