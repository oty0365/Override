using System;
using UnityEngine;

public enum SkillForm
{
    Passive,
    Active,
    Ultimate
}

public abstract class ACharacterSkill:MonoBehaviour
{
    public SkillForm skillForm;
    public abstract void UseSkill();
}
