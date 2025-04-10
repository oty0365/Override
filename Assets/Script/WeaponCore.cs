using TMPro;
using UnityEngine;

public class WeaponCore : MonoBehaviour
{
    public GameObject player;
    public GameObject weaponObject;
    private SpriteRenderer _weaponApearance;
    private WeaponBase _weapon;

    private void Start()
    {
        _weaponApearance = weaponObject.GetComponent<SpriteRenderer>();
        _weapon = weaponObject.GetComponent<WeaponBase>();
    }

    void Update()
    {
        gameObject.transform.position = player.transform.position;
        var dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - player.transform.position;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
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
    public void EndAttack()
    {
        _weaponApearance.enabled = false;
    }
}
