using UnityEditor;
using UnityEngine;

public class SwordOfVados: AAttack
{
    public Animator curAnimation;
    public APoolingObject blade;
    public Vector2 curDir;
    public float curRotation;
    private AnimatorStateInfo _stateInfo;
    private bool hadTrownBlade;
    public bool isChild;
    public override void OnBirth()
    {
        hadTrownBlade = false;
    }
    public override void OnDeathInit()
    {
        
    }
    void Start()
    {
        
    }
    void Update()
    {
        if (isChild)
        {
            return;
        }
        gameObject.transform.position = (Vector2)PlayerMove.Instance.gameObject.transform.position+curDir;
        _stateInfo = curAnimation.GetCurrentAnimatorStateInfo(0);
        if (!hadTrownBlade&&_stateInfo.normalizedTime >= 0.54f)
        {
            hadTrownBlade = true;
            ObjectPooler.Instance.Get(blade, gameObject.transform.position, new Vector3(0, 0,curRotation));
        }
        if (_stateInfo.normalizedTime >= 1f)
        {
            Death();
        }
        
    }
    
}
