using System;
using System.Collections.Generic;
using UnityEngine;

public enum CurrentSwippingWeapon
{
    BloodKantana = 0,
    Shovle,
    VoidSword,
    GreatSword
}

[Serializable]
public class WeaponSpinerSet
{
    public CurrentSwippingWeapon currentWeapon;
    public WeaponSpiner weaponSpinner;
}

public class WeaponSwipSkill : ACharacterSkill
{
    private static CurrentSwippingWeapon _currentWeapon;
    public static CurrentSwippingWeapon CurrentWeapon
    {
        get => _currentWeapon;
        set
        {
            if (value != _currentWeapon)
            {
                delWeapon?.Invoke();
                _currentWeapon = value;
                addWeapon?.Invoke();
            }
        }
    }

    public static Action delWeapon;
    public static Action addWeapon;

    public InstantinateModule instantinateModule;
    public AAttack currentWeaponSpiner;
    public WeaponSwip currentWeaponSlot;

    public WeaponSpinerSet[] weaponSpinners;
    public Dictionary<CurrentSwippingWeapon, WeaponSpiner> weaponDict = new();

    private PlayerInfo playerInfo;

    private void Start()
    {
        playerInfo = PlayerInfo.Instance;

        foreach (var i in weaponSpinners)
            weaponDict.Add(i.currentWeapon, i.weaponSpinner);

        delWeapon += DelWeapon;
        addWeapon += AddWeapon;

        UpdateSkill();
        UseSkill();

        CurrentWeapon = CurrentSwippingWeapon.Shovle;
    }

    public void DelWeapon()
    {
        currentWeaponSpiner?.Death();
    }

    public void AddWeapon()
    {
        if (!weaponDict.TryGetValue(CurrentWeapon, out var spinnerPrefab)) return;

        currentWeaponSpiner = ObjectPooler.Instance.Get(spinnerPrefab, currentWeaponSlot.weaponSlot.transform).GetComponent<WeaponSpiner>();
        //currentWeaponSpiner.gameObject.SetActive(true);
    }

    public override void UseSkill()
    {
        currentWeaponSlot = ObjectPooler.Instance.Get(instantinateModule.attackObj, playerInfo.transform.position, Vector3.zero).GetComponent<WeaponSwip>();
    }

    private void OnDestroy()
    {
        delWeapon -= DelWeapon;
        addWeapon -= AddWeapon;

        if (currentWeaponSlot != null)
        {
            currentWeaponSlot.Death();
        }

    }
}
