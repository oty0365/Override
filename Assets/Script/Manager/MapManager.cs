using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.UIElements;

public enum Dimention
{
    Normal,
    Code
}

public enum MapCode
{
    None,
    DreamersPrison,
    OriginsLand,
    DarkLoardsCastle,
    SkyHighFalls,
    RootNode,
    AtColony,
    SecurityCore,

}

public class MapManager : HalfSingleMono<MapManager>
{
    public Action mapChange;
    [Header("∏  ±‚∫ª ∞Ê∑Œ")]
    public string basicPath;
    [Header("∏  ¿˙¿Â ∞Ê∑ŒµÈ")]
    [SerializeField]private MapDatas mapDatas;

    public HashSet<string> mapSets = new();


    [Header("∏  UI")]
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private UnityEngine.UI.Slider progressBar;
    [SerializeField] private TextMeshProUGUI tmiText;

    public GameObject banner;
    public UnityEngine.UI.Image header;
    public UnityEngine.UI.Image endl;
    public TextMeshProUGUI mapNameTmp;
    public MapData currentMap;


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
                mapNameTmp.text = Scripter.Instance.scripts[value.ToString()].currentText;

                StartCoroutine(MapNameFlow());
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
    protected override void Awake()
    {
        base.Awake();
        foreach( var i in mapDatas.maps)
        {
            mapSets.Add(i);
        }
    }
    private void Start()
    {
        _playerCamera = PlayerCamera.Instance;
        PlayerInteraction.Instance.OnInteractMode(0);
        LoadMap("DreamersPrison-1");

    }
    /* private void Update()
     {
         if (Input.GetKeyDown(KeyCode.Space))
         {
             CurrentDimention = Dimention.Code;
         }
     }*/
    public void LoadMap(string address)
    {
        StartCoroutine(LoadWithProgressBar(address));
    }
    IEnumerator LoadWithProgressBar(string adress)
    {
        loadingPanel.SetActive(true);
        progressBar.value = 0;

        var handle = Addressables.LoadAssetAsync<MapData>(adress);
        var index = UnityEngine.Random.Range(1, 5);
        tmiText.text = Scripter.Instance.scripts["TMI-" + index].currentText;
        while (!handle.IsDone)
        {
            progressBar.value = handle.PercentComplete;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                index = UnityEngine.Random.Range(1, 5);
                tmiText.text = Scripter.Instance.scripts["TMI-" + index].currentText;
            }
            yield return new WaitForSecondsRealtime(Time.deltaTime);
        }
        progressBar.value = 0.7f;
        yield return new WaitForSeconds(0.1f);
        var head = Addressables.LoadAssetAsync<Sprite>(handle.Result.mapCode.ToString()+"Image");
        while (!head.IsDone)
        {
            progressBar.value = 0.7f+(head.PercentComplete*0.3f);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                index = UnityEngine.Random.Range(1, 5);
                tmiText.text = Scripter.Instance.scripts["TMI-" + index].currentText;
            }
            yield return new WaitForSecondsRealtime(Time.deltaTime);
        }
        header.sprite = Instantiate(head.Result);
        progressBar.value = 1;
        Instantiate(handle.Result.mapPrefab, handle.Result.spawnVector, Quaternion.identity);
        PlayerInfo.Instance.gameObject.transform.position = handle.Result.playerSpawn;
        yield return new WaitForSeconds(1.5f);
        PlayerInteraction.Instance.OnInteractMode(1);
        loadingPanel.SetActive(false);
        CurrentMapCode = handle.Result.mapCode;
        currentMap = Instantiate(handle.Result);
        Addressables.Release(handle);
        Addressables.Release(head);
    }

    IEnumerator MapNameFlow()
    {
        banner.SetActive(true);
        header.color = Color.clear;
        endl.color = Color.clear;
        mapNameTmp.color = Color.clear;
        for(var i = 0f; i < 1f; i += Time.deltaTime)
        {
            var a = Color.Lerp(header.color, Color.white,Time.deltaTime*3f);
            header.color = a;
            endl.color = a;
            mapNameTmp.color = a;
            yield return null;
        }
        yield return new WaitForSeconds(0.7f);
        header.color = Color.white;
        endl.color = Color.white;
        mapNameTmp.color = Color.white;
        for (var i = 0f; i < 1f; i += Time.deltaTime)
        {
            var a = Color.Lerp(header.color, Color.clear, Time.deltaTime*3f);
            header.color = a;
            endl.color = a;
            mapNameTmp.color = a;
            yield return null;
        }
        header.color = Color.clear;
        endl.color = Color.clear;
        mapNameTmp.color = Color.clear;
        banner.SetActive(false);
    }
}
