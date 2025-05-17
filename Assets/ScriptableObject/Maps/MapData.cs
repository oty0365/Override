using UnityEngine;

[CreateAssetMenu(fileName = "MapData", menuName = "Scriptable Objects/MapData")]
public class MapData : ScriptableObject
{
    public GameObject mapPrefab;
    public MapCode mapCode;
    public string mapSubName;
    public int mapLevel;
    public int mapHardness;
}
