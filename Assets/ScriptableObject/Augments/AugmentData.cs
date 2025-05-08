using UnityEngine;

public enum AugmentType 
{
    HpUp,
    AttackUp,
    SkillCoolUp,
    FullHp,
}

[CreateAssetMenu(fileName = "AugmentData", menuName = "Scriptable Objects/AugmentData")]
public class AugmentData : ScriptableObject
{
    public Sprite augmentSprite;
    public string augmentName;
    public AugmentType augmentType;
    [TextArea] public string augmentDesc;
    public InGameEvent[] effect;
}
