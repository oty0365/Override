using UnityEngine;

public class AtInCage : AInteractable
{
    [Header("���")]
    public Dialogs dialogs;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public override void OnInteract()
    {
        PlayerInteraction.Instance.OnInteractMode(0);
        DialogManager.Instance.StartConversation(dialogs);
    }
}
