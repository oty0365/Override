using TMPro;
using UnityEngine;

public class WeaponCore : MonoBehaviour
{
    public static WeaponCore Instance { get; private set; }

    public GameObject player;
    public GameObject weaponObject;
    public GameObject weaponSlot;
    public Vector2 mouseDir;
    public float rotaion;
    private SpriteRenderer _weaponApearance;
    private WeaponBase _weapon;

    private void Awake()
    {
        Instance = this;
    }

    public void ChangeWeapon(GameObject weapon)
    {
        PlayerCamera.Instance.SetZoom(4.5f, 8f);
        if (weaponSlot.transform.childCount != 0)
        {
            Destroy(weaponSlot.transform.GetChild(0).gameObject);
        }
        var w =Instantiate(weapon, weaponSlot.transform);
        weaponObject = w;
        _weaponApearance = weaponObject.GetComponent<SpriteRenderer>();
        _weapon = weaponObject.GetComponent<WeaponBase>();
        weaponSlot.transform.localPosition = new Vector3(_weapon.range,weaponSlot.transform.localPosition.y,weaponSlot.transform.localPosition.z);
    }

    void Update()
    {
        gameObject.transform.position = player.transform.position;
        mouseDir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - player.transform.position;
        rotaion = Mathf.Atan2(mouseDir.y, mouseDir.x) * Mathf.Rad2Deg;
        if(weaponObject != null)
        {
            if (_weapon.isAttacking)
            {
                if (mouseDir.x > 0)
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
                gameObject.transform.rotation = Quaternion.Euler(0, 0, rotaion);
            }
        }

    }
    public void EndAttack()
    {
        _weaponApearance.enabled = false;
    }
}
