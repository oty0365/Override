using UnityEngine;

public class StaticLayzer : ADamageable
{
    public override void OnHit()
    {
        if (CheckShield())
        {
            return;

        }

        GiveDamage(damage);
        PlayerMove.Instance.KnockBack(((Vector2)PlayerInfo.Instance.gameObject.transform.position - contactPoint).normalized, 14f, 0.1f);
        ObjectPooler.Instance.Get(hitParticle, contactPoint, new Vector3(0, 0, 0), new Vector2(1.5f, 1.5f));
        PlayerCamera.Instance.SetDamge(Speed * infinateTime, 0.3f);
        PlayerCamera.Instance.SetShake(0.35f, 7.5f, 0.6f);
        PlayerInfo.Instance.SetInfiniteTime(infinateTime);
    }
    public override void OnBirth()
    {
    }
    public override void OnDeathInit()
    {
    }

}
