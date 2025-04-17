using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiWeapon : MonoBehaviour
{
    [SerializeField] private WeaponData weapon;
    [SerializeField] private Image weaponImage;
    [SerializeField] private Image weaponFrame;
    [SerializeField] private TextMeshProUGUI weaponName;
    [SerializeField] private TextMeshProUGUI weaponDesc;
    [SerializeField] private TextMeshProUGUI weaponDamage;
    [SerializeField] private TextMeshProUGUI attackDelay;
     
    void Start()
    {
        weaponImage.sprite= weapon.weaponSprite;
        weaponFrame.color = weapon.weaponColor;
        weaponName.text = weapon.weaponName;
        weaponDesc.text = weapon.weaponDesc;
        weaponDamage.text = weapon.weaponStartDamage.ToString();
        attackDelay.text = weapon.weaponAttackSpeed.ToString();
    }
}
