using UnityEngine;

public class ChangeWeaponSkill : ACharacterSkill
{
    public void Start()
    {
        UpdateSkill();
    }
    public void Update()
    {
        if (Input.GetKeyDown(_currentKey)&&SkillManager.Instance.CheckToUseSkill(skillForm))
        {
            UseSkill();
        }
    }
    public override void UseSkill()
    {
        var r = Random.Range(0, 4);
        WeaponSwipSkill.CurrentWeapon = (CurrentSwippingWeapon)r;
        SkillManager.Instance.StartSkillCooldown(skillForm);
    }

}
