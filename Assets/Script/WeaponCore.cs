using TMPro;
using UnityEngine;

public class WeaponCore : MonoBehaviour
{
    public static WeaponCore Instance { get; private set; }

    public GameObject player;
    public GameObject weaponObject;
    public GameObject weaponSlot;
    private SpriteRenderer _weaponApearance;
    private WeaponBase _weapon;

    private void Awake()
    {
        Instance = this;
    }

    public void ChangeWeapon(GameObject weapon)
    {
        var w =Instantiate(weapon, weaponSlot.transform);
        weaponObject = w;
        _weaponApearance = weaponObject.GetComponent<SpriteRenderer>();
        _weapon = weaponObject.GetComponent<WeaponBase>();
    }

    void Update()
    {
        gameObject.transform.position = player.transform.position;
        var dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - player.transform.position;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if(weaponObject != null)
        {
            if (_weapon.isAttacking)
            {
                if (dir.x > 0)
                {
                    _weaponApearance.flipY = false;
                }
                else
                {
                    _weaponApearance.flipY = true;
                }
            }
            else
            {
                gameObject.transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }

    }
    public void EndAttack()
    {
        _weaponApearance.enabled = false;
    }
}
