using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapUploader : MonoBehaviour
{
    [SerializeField] private Tilemap tileMap;

    private void Awake()
    {
        TileMapManager.Instance.SetMap(tileMap);
    }
}
