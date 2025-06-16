using System.Collections;
using UnityEngine;

public class JellyBall :APoolingObject
{
    [SerializeField] private Animator ani;
    [SerializeField] private Collider2D col2D;
    [SerializeField] private AnimationClip desolveAni;
    private float _animationPlayTime;
    void Start()
    {
        
    }

    void Update()
    {

    }
    public override void OnBirth()
    {
        col2D.enabled = true;
        ani.Play("JellyBallIdel");
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _animationPlayTime = desolveAni.length;
            col2D.enabled = false;
            PlayerInfo.Instance.PlayerCurHp++;
            StartCoroutine(DeathFlow(_animationPlayTime));
        }
        
    }
    IEnumerator DeathFlow(float time)
    {
        ani.SetTrigger("break");
        yield return new WaitForSeconds(time);
        Death();
    }
    public override void OnDeathInit()
    {
    }
}
