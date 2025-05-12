using UnityEngine;

public class StaticLayzer : ADamageable
{
    public override void OnHit()
    {
        PlayerInfo.Instance.PlayerCurHp -= damage;
        PlayerMove.Instance.KnockBack(((Vector2)PlayerInfo.Instance.gameObject.transform.position - contactPoint).normalized, 6f, 0.3f);
        PlayerCamera.Instance.SetShake(0.35f, 8, 0.8f);
        PlayerInfo.Instance.SetInfinateTime(infinateTime);
    }

}
