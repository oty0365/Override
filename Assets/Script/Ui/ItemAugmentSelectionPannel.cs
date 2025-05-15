using UnityEngine;

public class ItemAugmentSelectionPannel : MonoBehaviour
{
    private Animator _ani;
    private GameObject _deleatalbeGameObj;

    void Start()
    {
        _ani=GetComponent<Animator>();
        _ani.updateMode = AnimatorUpdateMode.UnscaledTime;
        GameEventManager.Instance.UploadEvent(InGameEvent.AugmentOrItemSelection, OnSelectionStart);
    }

    void Update()
    {
        
    }

    public void OnSelectionStart()
    {
        _ani.SetTrigger("Start");
    }
    public void OnConfirm()
    {
        _ani.SetTrigger("End");
        Time.timeScale = 1;
    }
    public void OnSelection(int code)
    {
        AugmentManager.Instance.ActiveAugment(code);
        OnConfirm();
    }
    public void OnSelectionDemon()
    {
        OnSelectionStart();

    }


}
