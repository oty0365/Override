using UnityEngine;

public class WeaponSelectionPannel : MonoBehaviour
{
    private Animator _ani;

    void Start()
    {
        _ani=GetComponent<Animator>();
        GameEventManager.Instance.UploadEvent(InGameEvent.WeaponSelection, OnSelectionStart);
    }

    void Update()
    {
        
    }

    public void OnSelectionStart()
    {
        _ani.SetTrigger("Start");
    }
    public void OnQuitOrConfirm()
    {
        DialogManager.Instance.isEventing = false;
        DialogManager.Instance.NextText();
        _ani.SetTrigger("End");
    }
    public void OnSelectWeapon(int code)
    {
        PlayerInfo.Instance.PlayerWeapon = (WeaponCode)code;
        OnQuitOrConfirm();
    }



}
