using UnityEngine;

public class AtInCage : AInteractable,IHitable
{
    [Header("���")]
    public Dialogs dialogs;
    void Start()
    {
        
    }

    void Update()
    {
        
    }
    
    public void OnHit()
    {

    }

    public override void OnInteract()
    {
        PlayerInteraction.Instance.OnInteractMode(0);
        DialogManager.Instance.StartConversation(dialogs);
    }
}
