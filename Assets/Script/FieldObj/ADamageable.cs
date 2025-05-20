using UnityEngine;

public abstract class ADamageable : MonoBehaviour
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
        if (other.CompareTag("Player")&&!PlayerInfo.Instance.isInfinite)
        {
            contactPoint = other.ClosestPoint(transform.position);
            OnHit();
        }
    }
    protected void GiveDamage(float damage)
    {
        var playerInfo = PlayerInfo.Instance;
        if(playerInfo.PlayerDefence - damage >= 0)
        {
            playerInfo.PlayerDefence -= damage;
        }
        else
        {
            playerInfo.PlayerCurHp += playerInfo.PlayerDefence - damage;
        }
    }
    public abstract void OnHit();
}
