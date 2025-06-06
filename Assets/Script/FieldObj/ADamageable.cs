using UnityEngine;

public abstract class ADamageable : APoolingObject
{
    public float damage;
    public float infinateTime;
    [SerializeField]
    private float speed;

    public float Speed
    {
        get => speed == 0 ? 7f : speed; 
    }
    public Vector2 contactPoint;
    [SerializeField] protected APoolingObject hitParticle;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var playerInfo = PlayerInfo.Instance;
        if (other.CompareTag("Player")&&!playerInfo.isInfinite)
        {
            contactPoint = other.ClosestPoint(transform.position);
            OnHit();
        }
        OnColide();
        
    }
    protected void GiveDamage(float damage)
    {
        var playerInfo = PlayerInfo.Instance;
        if (playerInfo.PlayerDefence - damage >= 0)
        {
            playerInfo.PlayerDefence -= damage;
        }
        else
        {
            playerInfo.PlayerCurHp += playerInfo.PlayerDefence - damage;
        }
    }

    public virtual void OnColide() { }
    public abstract void OnHit();
    public bool CheckShield()
    {
        var playerInfo = PlayerInfo.Instance;
        if (playerInfo.shiledBuff.Count > 0)
        {
            playerInfo.shiledBuff.Pop().UseBuff();
            return true;
        }
        return false;
    }
}
