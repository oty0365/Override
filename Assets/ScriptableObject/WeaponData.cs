using UnityEngine;
[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/WeaponData")]
public class WeaponData : ScriptableObject
{
    public WeaponCode weaponCode;
    public WeaponDialogKey weaponName;
    public Sprite weaponSprite;
    public GameObject weaponPrefab;
    public WeaponDialogKey weaponDesc;
    public float weaponRange;
    public float weaponStartDamage;
    public float weaponAttackSpeed;
    public Color weaponColor;
}
