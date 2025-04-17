using System.Collections.Generic;
using UnityEngine;

public enum WeaponCode
{
    SwordShiled=0,
    Katana,
    GreatSword,
    MagicStaf,
    Spear
}

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; private set; }

    [Header("무기 배열")]
    public WeaponSets weaponSets;

    public Dictionary<WeaponCode, WeaponData> weaponDict = new Dictionary<WeaponCode, WeaponData>();

    private void Awake()
    {
        Instance = this;
        foreach(var i in weaponSets.weaponDatas)
        {
            weaponDict.Add(i.weaponCode, i);
        }
    }



}
