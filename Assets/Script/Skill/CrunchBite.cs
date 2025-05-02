using UnityEditor;
using UnityEngine;

public class CrunchBite : APoolingObject
{
    public Animator curAnimation;
    public Vector2 curDir;
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
        gameObject.transform.position = (Vector2)PlayerMove.Instance.gameObject.transform.position+curDir;
        _stateInfo = curAnimation.GetCurrentAnimatorStateInfo(0);
        if (_stateInfo.normalizedTime >= 1f)
        {
            Death();
        }
    }
}
