using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class ObjGenerator : HalfSingleMono<ObjGenerator>
{
    public ObjectGrenerateList publicGenerateList;
    private ObjectGrenerateList _privateGenerateList;
    public ObjectGrenerateList PrivateGenerateList
    {
        get => _privateGenerateList;
        set
        {
            if(value != null)
            {
                UpLoadToDict(value.objSet);
            }
        }
    }

    public Dictionary<string, APoolingObject> generateDict = new();

    private void UpLoadToDict(ObjSet[] objSet)
    {
        foreach (var i in objSet)
        {
            generateDict.Add(i.objCode, i.poolingObject);
        }
    }
    protected override void Awake()
    {
        base.Awake();
        UpLoadToDict(publicGenerateList.objSet);
    }
    void Start()
    {
        if(PrivateGenerateList != null)
        {
            UpLoadToDict(PrivateGenerateList.objSet);
        }
    }

    void Update()
    {
        
    }
}
