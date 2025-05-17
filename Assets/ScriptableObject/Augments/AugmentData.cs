using UnityEngine;

[CreateAssetMenu(fileName = "AugmentData", menuName = "Scriptable Objects/AugmentData")]
public class AugmentData : ScriptableObject
{
    public Sprite augmentSprite;
    public string augmentName;
    public string augmentDesc;
    public InGameEvent[] effect;
}
