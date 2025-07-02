using UnityEngine;

public class WeaponSelectionPannel : MonoBehaviour
{
    public static bool isSelected;
    private Animator _ani;
    private bool _hasSelectedWeapon = false;

    void Start()
    {
        _ani = GetComponent<Animator>();
        _ani.updateMode = AnimatorUpdateMode.UnscaledTime;
        GameEventManager.Instance.UploadEvent(InGameEvent.WeaponSelection, OnSelectionStart);
    }

    void Update()
    {

    }

    public void OnSelectionStart()
    {
        isSelected = false;
        _hasSelectedWeapon = false; 
        _ani.SetTrigger("Start");
    }

    public void OnQuitOrConfirm()
    {
        isSelected = true;
        DialogManager.Instance.isEventing = false;
        DialogManager.Instance.NextText();
        _ani.SetTrigger("End");
    }

    public void OnSelectWeapon(int code)
    {
        if (_hasSelectedWeapon)
        {
            return;
        }

        if (!isSelected)
        {
            PlayerInfo.Instance.PlayerWeapon = (WeaponCode)code;
            _hasSelectedWeapon = true;
        }

        OnQuitOrConfirm();
    }
}