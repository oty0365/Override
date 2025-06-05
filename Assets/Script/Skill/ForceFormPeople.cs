using UnityEngine;

public class ForceFormPeople :APoolingObject,Buff
{
    private PlayerInfo playerInfo;
    public float infinateTime;
    public ForceFromPeopleSkill manager;
    void Start()
    {
        
    }

    void Update()
    {
        gameObject.transform.position = playerInfo.gameObject.transform.position;
    }
    public override void OnBirth()
    {
        playerInfo = PlayerInfo.Instance;
    }
    public override void OnDeathInit()
    {
        manager.currentShield = null;
        manager = null;
    }
    public void UseBuff()
    {
        PlayerInfo.Instance.SetInfiniteTime(infinateTime);
        Death();
    }
}
