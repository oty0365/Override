
using UnityEngine;

public class WeaponSwip : APoolingObject
{
    private PlayerInfo playerInfo;
    public GameObject weaponSlot;  
    public float rotationSpeed = 500f; 
    private float currentAngle;

    void Update()
    {
        currentAngle = (currentAngle + (Time.deltaTime * rotationSpeed)) % 360;
        gameObject.transform.position = playerInfo.transform.position;
        gameObject.transform.rotation = Quaternion.Euler(0, 0, currentAngle);
    }

    public override void OnBirth()
    {
        playerInfo = PlayerInfo.Instance;
        for(var i = 0; i < weaponSlot.transform.childCount; i++)
        {
            weaponSlot.transform.GetChild(i).gameObject.SetActive(false);
        }
        currentAngle = 0f;
    }

    public override void OnDeathInit()
    {

    }
}
