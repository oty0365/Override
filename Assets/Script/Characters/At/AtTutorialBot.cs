using UnityEngine;

public class AtTutorialBot : AInteractable,IHitable
{
    [Header("ด๋ป็")]
    public Dialogs dialogs;

    public GameObject makeInteracter;

    private void Awake()
    {
        if(makeInteracter != null)
        {
            makeInteracter.layer = LayerMask.NameToLayer("Default");
        }
    }

    public void OnHit()
    {
    }
    public override void ExitInteract()
    {
        
    }
    public override void OnInteract()
    {
        if (makeInteracter != null)
        {
            makeInteracter.layer = LayerMask.NameToLayer("Interactable");
            makeInteracter = null;
        }
        PlayerInteraction.Instance.OnInteractMode(0);
        DialogManager.Instance.StartConversation(dialogs);
    }
}
