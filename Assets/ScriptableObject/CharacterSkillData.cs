using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSkillData", menuName = "Scriptable Objects/CharacterSkillData")]
public class CharacterSkillData : ScriptableObject
{
    public List<SkillData> identitySkills;
    public List<SkillData> ultimateSkills;

    public CharacterSkill characterSkill;
}
