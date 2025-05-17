using UnityEngine;

public class ForceFromPeopleSkill : ACharacterSkill
{
    public InstantinateModule instantinateModule;
    private PlayerInfo playerInfo;
    public void Start()
    {
        playerInfo = PlayerInfo.Instance;
        UpdateSkill();
    }
    public void Update()
    {
        if (SkillManager.Instance.CheckToUseSkill(skillForm)&&playerInfo.PlayerDefence<40)
        {
            UseSkill();
        }
    }
    public override void UseSkill()
    {
        if (playerInfo.PlayerDefence + 10 > 40)
        {
            playerInfo.PlayerDefence += ((playerInfo.PlayerDefence + 10) - 40);
        }
        else
        {
            playerInfo.PlayerDefence += 10f;
        }
        SkillManager.Instance.StartSkillCooldown(skillForm);
    }

}
