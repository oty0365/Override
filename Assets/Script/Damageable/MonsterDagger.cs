using UnityEngine;

public class MonsterDagger: ADamageable
{
    [SerializeField] private Animator ani;
    private AnimatorStateInfo currState;
    private void Update()
    {
        currState = ani.GetCurrentAnimatorStateInfo(0);
        if (currState.normalizedTime >= 1f)
        {
            Death();
        }
    }
    public override void OnHit()
    {
        if (CheckShield())
        {
            return;
        }

        GiveDamage(damage);
        PlayerMove.Instance.KnockBack(((Vector2)PlayerInfo.Instance.gameObject.transform.position - contactPoint).normalized, 7f, 0.1f);
        ObjectPooler.Instance.Get(hitParticle, contactPoint, new Vector3(0, 0, 0), new Vector2(1.5f, 1.5f));
        PlayerCamera.Instance.SetDamge(Speed * infinateTime, 0.3f);
        PlayerCamera.Instance.SetShake(0.3f, 5.5f, 0.4f);
        PlayerInfo.Instance.SetInfiniteTime(infinateTime);
    }
    public override void OnBirth()
    {
    }
    public override void OnDeathInit()
    {
    }

}
