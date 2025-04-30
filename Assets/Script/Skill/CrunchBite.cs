using UnityEditor;
using UnityEngine;

public class CrunchBite : APoolingObject
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
        _stateInfo = curAnimation.GetCurrentAnimatorStateInfo(0);
        if (_stateInfo.normalizedTime >= 1f)
        {
            Debug.Log("Å¬¸³ ³¡³µ¾î!");
            Death();
        }
    }
}
