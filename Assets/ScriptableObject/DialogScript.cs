using UnityEngine;

[CreateAssetMenu(fileName = "DialogScript", menuName = "Scriptable Objects/DialogScript")]
public class DialogScript : ScriptableObject
{
    public string talker;
    public bool hasSelection;
    public Sprite talkersFace;
    [TextArea]
    public string dialogue;
}
