using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D basicCursor;
    void Start()
    {
        Vector2 hotspot = new Vector2(basicCursor.width / 2, basicCursor.height / 2);
        Cursor.SetCursor(basicCursor, hotspot, CursorMode.Auto);
    }
}
