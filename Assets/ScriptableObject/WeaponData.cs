using UnityEngine;
[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/WeaponData")]
public class WeaponData : ScriptableObject
{
    public WeaponCode weaponCode;
    public string weaponName;
    public Sprite weaponSprite;
    [TextArea] public string weaponDesc;
    public float weaponRange;
    public float weaponStartDamage;
    public float weaponAttackSpeed;
    public Color weaponColor;
}
