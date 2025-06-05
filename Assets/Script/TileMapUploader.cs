using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapUploader : MonoBehaviour
{
    [SerializeField] private Tilemap tileMap;
    [SerializeField] private GameObject baseMap;

    private void Awake()
    {
        TileMapManager.Instance.SetMap(tileMap,baseMap);
    }
}
