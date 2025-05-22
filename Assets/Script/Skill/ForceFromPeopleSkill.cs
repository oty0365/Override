using UnityEngine;

public class ForceFromPeopleSkill : ACharacterSkill
{
    public InstantinateModule instantinateModule;
    public APoolingObject currentShield;
    private PlayerInfo playerInfo;
    public void Start()
    {
        playerInfo = PlayerInfo.Instance;
        UpdateSkill();
    }
    public void Update()
    {
        if (currentShield == null && SkillManager.Instance.CheckToUseSkill(skillForm))
        {
            UseSkill();
        }
    }
    public override void UseSkill()
    {
        var o = ObjectPooler.Instance.Get(instantinateModule.attackObj, PlayerInfo.Instance.gameObject.transform.position, new Vector3(0, 0, 0));
        playerInfo.shiledBuff.Push(o.GetComponent<Buff>());
        o.GetComponent<ForceFormPeople>().manager = this;
        currentShield = o.GetComponent<APoolingObject>();
        SkillManager.Instance.StartSkillCooldown(skillForm);
    }

}
