using UnityEngine;
using UnityEngine.Tilemaps;



public class CodeWorldObj : MonoBehaviour
{
    private TilemapRenderer _map;
    private MapManager mapManager;
    public Material[] materials;
    void Start()
    {
        _map = GetComponent<TilemapRenderer>();
        mapManager = MapManager.Instance;
        mapManager.mapChange += ChangeMat;
    }

    private void ChangeMat()
    {
        if (mapManager.CurrentDimention == Dimention.Normal)
        {
            _map.material = materials[0];
        }
        else
        {
            _map.material = materials[1];
        }

    }
    private void OnDestroy()
    {
        mapManager.mapChange -= ChangeMat;
    }
}
