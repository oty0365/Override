using System;
using System.Collections.Generic;
using UnityEngine;

public enum SkillForm
{
    PassiveIdentity,
    ActiveIdentity,
    PassiveUltimate,
    ActiveUltimate
}
public interface SkillMoudle
{
    
}
[System.Serializable]
public struct InstantinateModule:SkillMoudle
{
    public APoolingObject attackObj;
    public GameObject attackPos; 
}
[System.Serializable]
public struct EffectableModule : SkillMoudle
{
    public float amount;
}

public abstract class ACharacterSkill:MonoBehaviour
{
    public SkillForm skillForm;
    private float _skillCooldown;
    public float SkillCooldown {
        get => _skillCooldown;
        set
        {
            if(value <0)
            {
                value = 0;
            }
            _skillCooldown = value;
        }
    }

    public void UpdateSkillCooldown()
    {
        switch (skillForm)
        {
            case SkillForm.PassiveIdentity:
            case SkillForm.ActiveIdentity:
                SkillCooldown+=SkillManager.Instance.curridentitySkill.basicCoolDown;
                break;
            case SkillForm.PassiveUltimate:
            case SkillForm.ActiveUltimate:
                SkillCooldown += SkillManager.Instance.currultimateSkill.basicCoolDown;
                break;
        }
    }

    public abstract void UseSkill();
}
