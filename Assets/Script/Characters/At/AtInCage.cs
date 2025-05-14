using UnityEngine;

public class AtInCage : AInteractable,IHitable
{
    [Header("ด๋ป็")]
    public Dialogs dialogs;
    public Animator ani;

    public GameObject[] sealedObjects;
    void Start()
    {
        GameEventManager.Instance.UploadEvent(InGameEvent.AtDown, AtDown);
        foreach( var i in sealedObjects)
        {
            i.SetActive(false);
        }
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
        foreach (var i in sealedObjects)
        {
            i.SetActive(true);
        }
        DialogManager.Instance.StartConversation(dialogs);
    }
    public override void ExitInteract()
    {
       
    }
}
