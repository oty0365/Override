using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpawnMode
{
    Set = 0,
    NormalRandom =1,
    CurruptedRandom = -1,
    BossBattle = 2
}

public class BattleManager : MonoBehaviour
{
    public SpawnMode spawnMode;
    [SerializeField]
    private int _monsterCount;

    private Coroutine _currentStareFlow;
    public int MonsterCount
    {
        get => _monsterCount;
        set
        {
            _monsterCount = value;
            if (value <= 0)
            {
                _currentStareFlow=StartCoroutine(StareFlow());
                if (gameEndObj != null)
                {
                    gameEndObj.SetActive(true);
                }
                var index = UnityEngine.Random.Range(0, items.Length);
                Instantiate(items[index], new Vector2(gameEndObj.transform.position.x,gameEndObj.transform.position.y-2f),Quaternion.identity);

            }
            MapManager.Instance.CurrentMonsters = _monsterCount;
        }
    }
    [SerializeField] private APoolingObject[] normal;
    [SerializeField] private APoolingObject[] currupted;
    [SerializeField] private APoolingObject[] monsters;
    [SerializeField] private GameObject gameEndObj;
    public float spawnRate;
    public List<Transform> spawns;

    public List<GameObject> spawnedMonsters;
    [SerializeField] private GameObject[] items;
    private void Awake()
    {
        if (gameEndObj != null)
        {
            gameEndObj.SetActive(false);
        }
        if (MapManager.Instance.index < 5)
        {
            if(spawnMode==SpawnMode.NormalRandom)
            {
                spawnMode = (SpawnMode)((int)spawnMode * -1);
            }
        }
    }
    void Start()
    {
        MapManager.Instance.battleManager = this;
        for(var i  = 0; i < transform.childCount; i++)
        {
            spawns.Add(transform.GetChild(i));
        }

        switch (spawnMode)
        {
            case SpawnMode.NormalRandom:
            case SpawnMode.CurruptedRandom:
                MonsterCount = spawns.Count;
                RandomSpawn();
                break;

            case SpawnMode.Set:
                MonsterCount = spawns.Count;
                SetSpawn();
                break;

            case SpawnMode.BossBattle:
                MonsterCount = spawns.Count;
                break;

        }
    }
    private void RandomSpawn()
    {
        if(spawnMode < 0)
        {
            spawnRate = 1000f;
        }
        foreach (var spawn in spawns)
        {
            var i = UnityEngine.Random.Range(1, 101);
            if (i <= spawnRate)
            {
                var r = UnityEngine.Random.Range(0, normal.Length);
                var o = ObjectPooler.Instance.Get(normal[r], spawn.position, Vector3.zero);
                o.GetComponent<Enemy>().battleManager = this;
                spawnedMonsters.Add(o);
                o.gameObject.SetActive(true);
            }
            else
            {
                var r = UnityEngine.Random.Range(0, currupted.Length);
                var o = ObjectPooler.Instance.Get(currupted[r], spawn.position, Vector3.zero);
                o.GetComponent<Enemy>().battleManager = this;
                spawnedMonsters.Add(o);
                o.gameObject.SetActive(true);
            }
        }
    }

    private void SetSpawn()
    {
        for(var i = 0; i < spawns.Count; i++)
        {
            var o = ObjectPooler.Instance.Get(monsters[i], spawns[i].position, Vector3.zero);
            o.GetComponent<Enemy>().battleManager = this;
            spawnedMonsters.Add(o);
            o.gameObject.SetActive(true);
        }
    }
    public void ReturnAll()
    {
        foreach (var i in spawnedMonsters)
        {
            if (i != null)
            {
                i.GetComponent<Enemy>().RemovedByGame();
            }
        }
    }
    private IEnumerator StareFlow()
    {
        PlayerInteraction.Instance.OnInteractMode(0);
        PlayerCamera.Instance.target = gameEndObj;
        while (Vector2.Distance(gameEndObj.transform.position, PlayerCamera.Instance.transform.position) > 0.1f)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.85f);
        PlayerCamera.Instance.target = PlayerInfo.Instance.gameObject;
        while (Vector2.Distance(PlayerInfo.Instance.transform.position, PlayerCamera.Instance.transform.position) > 0.1f)
        {
            yield return null;
        }
        PlayerCamera.Instance.currentZoomSize = 4.5f;
        PlayerInteraction.Instance.OnInteractMode(1);
    }
    private void OnDestroy()
    {
        if (_currentStareFlow != null)
        {
            StopCoroutine(_currentStareFlow);
        }

        PlayerCamera.Instance.target = PlayerInfo.Instance.gameObject;
    }


}
