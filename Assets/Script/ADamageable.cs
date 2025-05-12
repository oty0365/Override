using UnityEngine;

public abstract class ADamageable : MonoBehaviour
{
    public float damage;
    public float infinateTime;
    public Vector2 contactPoint;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")&&!PlayerInfo.Instance.isInfinate)
        {
            contactPoint = other.ClosestPoint(transform.position);
            OnHit();
        }
    }

    public abstract void OnHit();
}
