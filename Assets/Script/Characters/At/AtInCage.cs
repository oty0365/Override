using UnityEngine;

public class AtInCage : AInteractable,IHitable
{
    [Header("ด๋ป็")]
    public Dialogs dialogs;
    public Animator ani;
    void Start()
    {
        GameEventManager.Instance.UploadEvent(InGameEvent.AtDown, AtDown);

    }

    void Update()
    {
        
    }

    public void AtDown()
    {
        ani.SetTrigger("Down");
        gameObject.layer = LayerMask.NameToLayer("Default");
        DialogManager.Instance.isEventing = false;
        DialogManager.Instance.NextText();
    }
    
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
