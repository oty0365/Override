using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum SpawnMode
{
    Set = 0,
    NormalRandom =1,
    CurruptedRandom = -1,
}

public class BattleManager : MonoBehaviour
{
    public SpawnMode spawnMode;
    [SerializeField]
    private int _monsterCount;
    public int MonsterCount
    {
        get => _monsterCount;
        set
        {
            _monsterCount = value;
            if (value <= 0)
            {
                if (gameEndObj != null)
                {
                    gameEndObj.SetActive(true);
                }

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
        for(var i  = 0; i < transform.childCount; i++)
        {
            spawns.Add(transform.GetChild(i));
        }

        MonsterCount = spawns.Count;
        switch (spawnMode)
        {
            case SpawnMode.NormalRandom:
            case SpawnMode.CurruptedRandom:
                RandomSpawn();
                break;

            case SpawnMode.Set:
                SetSpawn();
                break;

        }
        Debug.Log(spawnedMonsters.Count);
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
                Debug.Log(normal[r]);
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
    private void OnDestroy()
    {
        foreach(var i in spawnedMonsters)
        {
            if(i != null)
            {
                i.GetComponent<Enemy>().RemovedByGame();
            }
        }
    }
}
