using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public enum Dimention
{
    Normal,
    Code
}

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
    public Action mapChange;
    [Header("¸Ê UI")]
    public GameObject banner;
    public Image header;
    public Image endl;
    public TextMeshProUGUI mapNameTmp;

    private PlayerCamera _playerCamera;

    [SerializeField]
    private MapCode currentMapCode;
    public MapCode CurrentMapCode
    {
        get => currentMapCode;
        set
        {
            if (value != currentMapCode)
            {
                currentMapCode = value;
            }
        }
    }
    [SerializeField]
    private Dimention currentDimention;
    public Dimention CurrentDimention
    {
        get => currentDimention;
        set
        {
            currentDimention = value;
            if(value == Dimention.Normal)
            {
                _playerCamera.colorAdjustments.saturation.value = 0;
            }
            else
            {
                _playerCamera.colorAdjustments.saturation.value = -100;
            }
                mapChange.Invoke();
        }
    }
    private void Start()
    {
        _playerCamera = PlayerCamera.Instance;

    }
    

}
