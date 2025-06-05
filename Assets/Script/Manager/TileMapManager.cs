using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileInfo
{
    public TileBase tileType;
    public bool ableToGo;
}

public class TileMapManager : MonoBehaviour
{
    public static TileMapManager Instance { get; private set; }
    public Tilemap tilemap;
    public Dictionary<Vector3Int, TileInfo> mapInfos = new Dictionary<Vector3Int, TileInfo>();
    public GameObject floor;
    public GameObject wall;

    private void Awake()
    {
        Instance = this;
    }

    public void SetMap(Tilemap tileMap,GameObject baseMap)
    {
        tilemap = tileMap;
        mapInfos.Clear();
        BoundsInt bounds = tileMap.cellBounds;
        Vector3Int cellPos = new Vector3Int(bounds.xMin, bounds.yMin, 0);
        TileInfo tileInfo = new TileInfo();
        mapInfos.Add(cellPos, tileInfo);
        TileBase tile = tileMap.GetTile(cellPos);
        Vector3 worldPos = tileMap.CellToWorld(cellPos);
        baseMap.gameObject.transform.position += (cellPos- worldPos);



        for (int y = bounds.yMin; y < bounds.yMax; y++)
        {
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                cellPos = new Vector3Int(x, y, 0);
                tileInfo = new TileInfo();
                mapInfos.TryAdd(cellPos, tileInfo);

                tile = tileMap.GetTile(cellPos);
                worldPos = tileMap.CellToWorld(cellPos);
                worldPos += tileMap.cellSize * 0.5f; 

                if (tile == null)
                {
                    //Instantiate(floor, worldPos, Quaternion.identity);
                    mapInfos[cellPos].ableToGo = true;
                }
                else
                {
                    //Instantiate(wall, worldPos, Quaternion.identity);
                    mapInfos[cellPos].ableToGo = false;
                    mapInfos[cellPos].tileType = tile;
                }
            }
        }
    }



    void Start()
    {
    }
}