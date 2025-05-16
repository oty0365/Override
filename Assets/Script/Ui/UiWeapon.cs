using TMPro;
using Unity.VisualScripting;
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
        var scripter = Scripter.Instance;
        weaponImage.sprite= weapon.weaponSprite;
        weaponFrame.color = weapon.weaponColor;
        weaponName.text = scripter.scripts[weapon.weaponName.ToString()].currentText;
        weaponDesc.text = scripter.scripts[weapon.weaponDesc.ToString()].currentText;
        weaponDamage.text = weapon.weaponStartDamage.ToString();
        attackDelay.text = weapon.weaponAttackSpeed.ToString();
    }
}
