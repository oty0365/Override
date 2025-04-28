using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "Scriptable Objects/SkillData")]
public class SkillData : ScriptableObject
{
    public string skillName;
    public Sprite skillIcon;
    public float basicCoolDown;
    [TextArea] public string skillDesc;
}
