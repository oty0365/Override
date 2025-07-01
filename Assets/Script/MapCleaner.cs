using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class MapCleaner : HalfSingleMono<MapCleaner>
{

    public void Add(GameObject obj)
    {
        obj.transform.SetParent(gameObject.transform, true);
    }
    public void Clear()
    {
        for(var i = 0;i<gameObject.transform.childCount;i++)
        {
            Destroy(gameObject.transform.GetChild(i).gameObject);
        }
    }

   
}
