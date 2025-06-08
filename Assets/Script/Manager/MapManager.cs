using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
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
    [SerializeField] private MapDatas mapDatas;
    public List<MapData> mapList;
    public HashSet<string> mapSets = new();
    public List<string> initialList;

    [Header("∏  UI")]
    public GameObject loadingPanel;
    [SerializeField] private UnityEngine.UI.Slider progressBar;
    [SerializeField] private TextMeshProUGUI tmiText;
    [SerializeField] private GameObject bugLeftPannel;
    [SerializeField] private TextMeshProUGUI bugLeftText;

    public GameObject banner;
    public UnityEngine.UI.Image header;
    public UnityEngine.UI.Image endl;
    public TextMeshProUGUI mapNameTmp;
    public MapData currentMap;
    public GameObject currentMapObj;

    public int index;
    public bool isLoading;


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

    private int _currentMonsters;
    public int CurrentMonsters
    {
        get => _currentMonsters;
        set
        {
            if (value <= 0)
            {
                _currentMonsters = 0;
                bugLeftPannel.SetActive(false);
            }
            else
            {
                _currentMonsters = value;
                bugLeftPannel.SetActive(true);
            }
            bugLeftText.text = _currentMonsters.ToString();
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
        CurrentMonsters = 0;
        _playerCamera = PlayerCamera.Instance;
        PlayerInteraction.Instance.OnInteractMode(0);
        TutorialMap();
    }

    public void NextMap()
    {
        currentMap = mapList[++index];
        Destroy(currentMapObj);
        SetMap();
    }

    /* private void Update()
     {
         if (Input.GetKeyDown(KeyCode.Space))
         {
             CurrentDimention = Dimention.Code;
         }
     }*/
    
    public void TutorialMap()
    {
        initialList.Clear();
        initialList.Add("DreamersPrison-1");
        initialList.Add("DreamersPrison-2");
        StartCoroutine(LoadFlow());
    }

    public void SetGameMap(int start, int end, int midBoss, int finalBoss)
    {
        initialList.Clear();
        var areaMaps = new List<string>();
        for(int i = start; i <= end; i++)
        {
            areaMaps.Add(mapDatas.maps[i]);
        }
        for(int i = 0; i < 4; i++)
        {
            var index=UnityEngine.Random.Range(0, areaMaps.Count);
            initialList.Add(areaMaps[index]);
            areaMaps.Remove(areaMaps[index]);
        }
        initialList.Add(mapDatas.maps[midBoss]);
        for (int i = 0; i < 3; i++)
        {
            var index = UnityEngine.Random.Range(0, areaMaps.Count);
            initialList.Add(areaMaps[index]);
            areaMaps.Remove(areaMaps[index]);
        }
        initialList.Add(mapDatas.maps[finalBoss]);
        StartCoroutine(LoadFlow());
    }

    public void RootNodeMap()
    {
        initialList.Clear();
        initialList.Add("RootNode-1");
        StartCoroutine(LoadFlow());
    }
    IEnumerator LoadFlow()
    {
        mapList.Clear();
        if (currentMapObj != null)
        {
            Destroy(currentMapObj);
        }
        index = 0;
        yield return StartCoroutine(LoadMapsWithProgressBar(initialList));
        currentMap = mapList[0];
        SetMap();
        loadingPanel.SetActive(false);
    }

    public void SetMap()
    {
        header.sprite = currentMap.mapBanner;
        CurrentMapCode = currentMap.mapCode;
        currentMapObj = Instantiate(currentMap.mapPrefab, currentMap.spawnVector, Quaternion.identity);
        PlayerInfo.Instance.gameObject.transform.position = currentMap.playerSpawn;
        PlayerCamera.Instance.gameObject.transform.position = PlayerInfo.Instance.transform.position;
    }

    IEnumerator LoadMapsWithProgressBar(List<string> adress)
    {
        isLoading = true;
        loadingPanel.SetActive(true);
        progressBar.value = 0;
        var index = UnityEngine.Random.Range(1, 7);
        tmiText.text = Scripter.Instance.scripts["TMI-" + index].currentText;
        foreach (var i in adress)
        {
            var handle = Addressables.LoadAssetAsync<MapData>(i);
            while (!handle.IsDone)
            {
                //progressBar.value = handle.PercentComplete;
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    index = UnityEngine.Random.Range(1, 5);
                    tmiText.text = Scripter.Instance.scripts["TMI-" + index].currentText;
                }
                yield return new WaitForSecondsRealtime(Time.deltaTime);
            }
            mapList.Add(Instantiate(handle.Result));
            Addressables.Release(handle);
            progressBar.value += 1f / adress.Count;
            yield return new WaitForSeconds(0.07f);
        }
        progressBar.value = 1;
        yield return new WaitForSeconds(1f);
        PlayerInteraction.Instance.OnInteractMode(1);
        isLoading = false;

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
