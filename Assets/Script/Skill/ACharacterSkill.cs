using System;
using System.Collections.Generic;
using UnityEngine;

public enum SkillForm
{
    PassiveIdentity,
    ActiveIdentity,
    PassiveAt,
    ActiveAt,
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
    public Transform attackTransform;
    public float range;
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
    protected KeyCode _currentKey;

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

    public void UpdateSkill()
    {
        switch (skillForm)
        {
            case SkillForm.PassiveIdentity:
            case SkillForm.ActiveIdentity:
                SkillCooldown+=SkillManager.Instance.curridentitySkill.basicCoolDown;
                _currentKey = KeyBindingManager.Instance.keyBindings["SpecialAttack1"];
                break;
            case SkillForm.PassiveUltimate:
            case SkillForm.ActiveUltimate:
                SkillCooldown += SkillManager.Instance.currultimateSkill.basicCoolDown;
                _currentKey = KeyBindingManager.Instance.keyBindings["Ultiamte"];
                break;
        }
    }

    public abstract void UseSkill();

    
}
