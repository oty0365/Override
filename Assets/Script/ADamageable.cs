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

    public abstract void OnHit();
}
