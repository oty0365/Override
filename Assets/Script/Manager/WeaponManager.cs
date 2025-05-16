using System.Collections.Generic;
using UnityEngine;

public enum WeaponCode
{
    SwordShiled=0,
    Katana,
    GreatSword,
    MagicStaff,
    Dagger
}

public class WeaponManager : HalfSingleMono<WeaponManager>
{

    [Header("무기 배열")]
    public WeaponSets weaponSets;

    public Dictionary<WeaponCode, WeaponData> weaponDict = new Dictionary<WeaponCode, WeaponData>();

    protected override void Awake()
    {
        base.Awake();
        foreach(var i in weaponSets.weaponDatas)
        {
            weaponDict.Add(i.weaponCode, i);
        }
    }



}
