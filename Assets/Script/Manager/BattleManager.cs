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
            MapManager.Instance.CurrentMonsters = _monsterCount;
        }
    }
    [SerializeField] private APoolingObject[] normal;
    [SerializeField] private APoolingObject[] currupted;
    [SerializeField] private APoolingObject[] monsters;
    public float spawnRate;
    public List<Transform> spawns;
    private void Awake()
    {
        if (MapManager.Instance.index > 5)
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

            }
            else
            {
                var r = UnityEngine.Random.Range(0, currupted.Length);
                var o = ObjectPooler.Instance.Get(currupted[r], spawn.position, Vector3.zero);
                o.GetComponent<Enemy>().battleManager = this;
            }
        }
    }

    private void SetSpawn()
    {
        for(var i = 0; i < spawns.Count; i++)
        {
            var o = ObjectPooler.Instance.Get(monsters[i], spawns[i].position, Vector3.zero);
            o.GetComponent<Enemy>().battleManager = this;
        }
    }
}
