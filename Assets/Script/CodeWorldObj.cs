using UnityEngine;
using UnityEngine.Tilemaps;



public class CodeWorldObj : MonoBehaviour
{
    [SerializeField] private bool isGround;
    private TilemapRenderer _map;
    private Tilemap _tilemap;
    private MapManager mapManager;
    public Material[] materials;

    [SerializeField] private Color normal;
    [SerializeField] private Color code;
    void Start()
    {
        _map = GetComponent<TilemapRenderer>();
        _tilemap = GetComponent<Tilemap>();
        mapManager = MapManager.Instance;
        mapManager.mapChange += ChangeMat;
    }

    private void ChangeMat()
    {
        if (mapManager.CurrentDimention == Dimention.Normal)
        {
            _map.material = materials[0];
            _tilemap.color = normal;
        }
        else
        {
            _map.material = materials[1];
            if(isGround)
            {
                _tilemap.color = code;
            }
        }

    }
    private void OnDestroy()
    {
        mapManager.mapChange -= ChangeMat;
    }
}
