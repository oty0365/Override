using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum MapCode
{
    DreamersPrison,
    OriginsLand,
    DarkLoadsCastle,
    SkyHighFalls,
    RootNode,
    AtColony,
    SecurityCore,

}

public class MapManager : HalfSingleMono<MapManager>
{
    [Header("¸Ê UI")]
    public GameObject banner;
    public Image header;
    public Image endl;
    public TextMeshProUGUI mapNameTmp;
    private MapCode _currentMapCode;
    public MapCode CurrentMapCode
    {
        get => _currentMapCode;
        set
        {
            if (value != _currentMapCode)
            {
                _currentMapCode = value;
            }
        }
    }


}
