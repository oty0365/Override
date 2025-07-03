using UnityEngine;

public class TheScrollOfAugmentSkill : ACharacterSkill
{
    public void Start()
    {
        UpdateSkill();
    }
    public void Update()
    {
        if (Input.GetKeyDown(_currentKey)&&SkillManager.Instance.CheckToUseSkill(skillForm)&&PlayerInfo.Instance.isInBattle)
        {
            UseSkill();
        }
    }
    public override void UseSkill()
    {
        AugmentManager.Instance.RandomAugmentOutput();
        SkillManager.Instance.StartSkillCooldown(skillForm);
        Time.timeScale = 0;
        GameEventManager.Instance.eventsDict[InGameEvent.AugmentOrItemSelection].Invoke();
    }

}
