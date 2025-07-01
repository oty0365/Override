using UnityEngine;

public class MonsterArrow: ADamageable
{
    public Rigidbody2D rb;
    [SerializeField] private float arrowSpeed;
    [SerializeField] private TrailRenderer tr;
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
        PlayerCamera.Instance.SetShake(0.3f, 4.5f, 0.3f);
        PlayerInfo.Instance.SetInfiniteTime(infinateTime);
    }
    public override void OnBirth()
    {
        tr.emitting = true;
        rb.linearVelocity = transform.right * arrowSpeed;
    }
    public override void OnDeathInit()
    {
        tr.Clear();
        tr.emitting = false;
    }
    public override void OnColide()
    {
        base.OnColide();
        Death();
    }



}
