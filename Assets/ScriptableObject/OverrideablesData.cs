using UnityEngine;

public enum Rarity
{
    Common,
    Rare,
    Epic,
    Legendary,
    Unknown
}

[CreateAssetMenu(fileName = "OverrideablesData", menuName = "Scriptable Objects/OverrideablesData")]
public class OverrideablesData : ScriptableObject
{
    public string overridealbeName;
    public Rarity rarity;
    public AnimationClip[] animationClips;
    [TextArea]
    public string desc;
    public GameObject prefabObject;

}
