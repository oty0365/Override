using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileInfo
{
    public TileBase tileType;
    public bool ableToGo;
}

public class TileMapManager : HalfSingleMono<TileMapManager>
{
    public Dictionary<Vector3Int,TileInfo> mapInfos = new Dictionary<Vector3Int, TileInfo>();

    public void SetMap(Tilemap tileMap)
    {
        mapInfos.Clear();
        BoundsInt bounds = tileMap.cellBounds;

        for (int y = bounds.yMin; y < bounds.yMax; y++)
        {
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                TileInfo tileInfo = new TileInfo();
                mapInfos.Add(pos, tileInfo);
                TileBase tile = tileMap.GetTile(pos);
                if (tile==null)
                {
                    mapInfos[pos].ableToGo = true;
                }
                else
                {
                    mapInfos[pos].ableToGo = false;
                    mapInfos[pos].tileType = tile;
                }
            }
        }
        foreach(var i in mapInfos)
        {
            Debug.Log(i.Key+","+i.Value);
        }
    }

}
