using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(fileName = "AugmentDatas", menuName = "Scriptable Objects/AugmentDatas")]
public class AugmentDatas : ScriptableObject
{
    public AugmentData[] augments;
}
