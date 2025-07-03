using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
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

    public BattleManager battleManager;

    public int index;
    public bool isLoading;

    private List<AsyncOperationHandle<MapData>> mapHandles = new List<AsyncOperationHandle<MapData>>();

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
                SoundManager.Instance.PlayBGM(currentMapCode.ToString());
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
                PlayerInfo.Instance.isInBattle = false;
                bugLeftPannel.SetActive(false);
            }
            else
            {
                _currentMonsters = value;
                PlayerInfo.Instance.isInBattle = true;
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
            if (value == Dimention.Normal)
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
        foreach (var i in mapDatas.maps)
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
        for (int i = start; i <= end; i++)
        {
            areaMaps.Add(mapDatas.maps[i]);
        }
        for (int i = 0; i < 4; i++)
        {
            var index = UnityEngine.Random.Range(0, areaMaps.Count);
            initialList.Add(areaMaps[index]);
            areaMaps.Remove(areaMaps[index]);
        }
        for (int i = 0; i < 3; i++)
        {
            var index = UnityEngine.Random.Range(0, areaMaps.Count);
            initialList.Add(areaMaps[index]);
            areaMaps.Remove(areaMaps[index]);
        }
        initialList.Add(mapDatas.maps[midBoss]);
        StartCoroutine(LoadFlow());
    }

    public void RootNodeMap()
    {
        initialList.Clear();
        initialList.Add("RootNode-1");
        PlayerInfo.Instance.ClearStatus();
        StartCoroutine(LoadFlow());
    }

    IEnumerator LoadFlow()
    {
        ReleaseMaps();
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
        MapCleaner.Instance.Clear();
        header.sprite = currentMap.mapBanner;
        CurrentMapCode = currentMap.mapCode;
        currentMapObj = Instantiate(currentMap.mapPrefab, currentMap.spawnVector, Quaternion.identity);
        PlayerInfo.Instance.gameObject.transform.position = currentMap.playerSpawn;
        PlayerCamera.Instance.gameObject.transform.position = new Vector3(PlayerInfo.Instance.gameObject.transform.position.x, PlayerInfo.Instance.gameObject.transform.position.y, PlayerCamera.Instance.gameObject.transform.position.z);
    }

    IEnumerator LoadMapsWithProgressBar(List<string> adress)
    {
        isLoading = true;
        loadingPanel.SetActive(true);
        progressBar.value = 0;

        var index = UnityEngine.Random.Range(1, 7);
        tmiText.text = Scripter.Instance.scripts["TMI-" + index].currentText;

        mapList.Clear();

        foreach (var i in adress)
        {
            var handle = Addressables.LoadAssetAsync<MapData>(i);
            yield return handle;

            MapData mapData = handle.Result;
            mapList.Add(mapData);
            mapHandles.Add(handle);

            progressBar.value += 1f / adress.Count;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                index = UnityEngine.Random.Range(1, 5);
                tmiText.text = Scripter.Instance.scripts["TMI-" + index].currentText;
            }

            yield return new WaitForSecondsRealtime(0.07f);
        }

        progressBar.value = 1;
        yield return new WaitForSecondsRealtime(0.3f);

        PlayerInteraction.Instance.OnInteractMode(1);
        isLoading = false;
        PlayerInfo.Instance.InitializeStatus();
    }

    void ReleaseMaps()
    {
        foreach (var handle in mapHandles)
        {
            if (handle.IsValid())
                Addressables.Release(handle);
        }
        mapHandles.Clear();
    }

    IEnumerator MapNameFlow()
    {
        banner.SetActive(true);
        header.color = Color.clear;
        endl.color = Color.clear;
        mapNameTmp.color = Color.clear;
        for (var i = 0f; i < 1f; i += Time.deltaTime)
        {
            var a = Color.Lerp(header.color, Color.white, Time.deltaTime * 3f);
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
            var a = Color.Lerp(header.color, Color.clear, Time.deltaTime * 3f);
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

    void OnDestroy()
    {
        //ReleaseMaps();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            //ReleaseMaps();
        }
    }
}