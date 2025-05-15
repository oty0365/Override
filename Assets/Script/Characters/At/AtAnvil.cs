using UnityEngine;

public class AtAnvil : AInteractable,IHitable
{
    [Header("���")]
    public Dialogs dialogs;   
    public void OnHit()
    {

    }

    public override void OnInteract()
    {
        PlayerInteraction.Instance.OnInteractMode(0);
        DialogManager.Instance.StartConversation(dialogs);
    }
    public override void ExitInteract()
    {
       
    }
}
