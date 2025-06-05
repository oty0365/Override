using UnityEditor;
using UnityEngine;

public class BlazeOfEvil: AAttack
{
    public Animator curAnimation;
    private AnimatorStateInfo _stateInfo;
    public override void OnBirth()
    {
        
    }
    public override void OnDeathInit()
    {
    }
    void Start()
    {
        
    }
    void Update()
    {
        gameObject.transform.position = PlayerMove.Instance.gameObject.transform.position;
        _stateInfo = curAnimation.GetCurrentAnimatorStateInfo(0);
        if (_stateInfo.normalizedTime >= 1f)
        {
            Death();
        }
    }
}
