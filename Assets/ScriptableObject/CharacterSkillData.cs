using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillSets
{
    public SkillData skillData;
    public ACharacterSkill characterSkill;
}

[CreateAssetMenu(fileName = "CharacterSkillData", menuName = "Scriptable Objects/CharacterSkillData")]
public class CharacterSkillData : ScriptableObject
{
    public List<SkillSets> identitySkills;
    public List<SkillSets> ultimateSkills;

}
