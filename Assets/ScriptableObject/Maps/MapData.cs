using UnityEngine;
using UnityEngine.Tilemaps;


[CreateAssetMenu(fileName = "MapData", menuName = "Scriptable Objects/MapData")]
public class MapData : ScriptableObject
{
    public Sprite mapBanner;
    public GameObject mapPrefab;
    public MapCode mapCode;
    public string mapSubName;
    public int mapLevel;
    public int mapHardness;
    public Vector2 spawnVector;
    public Vector2 playerSpawn;

}
