using NUnit.Framework.Constraints;
using System.Collections;
using UnityEngine;

public class SoulBullet: AAttack
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private TrailRenderer tr;
    [SerializeField] private float fadeSpeed;
    [SerializeField] private float shootSpeed;
    [SerializeField] private float waitTime;
    [SerializeField] APoolingObject colideParticle;
    public override void OnBirth()
    {
        tr.Clear();
        tr.emitting = true;
        sr.color = new Color(1, 1, 1, 1);
        rb.linearVelocity = transform.right.normalized*shootSpeed;
        StartCoroutine(DaggerRemainFlow());
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        rb.linearVelocity = Vector2.zero;
        Vector2 contactPoint = other.ClosestPoint(transform.position);
        ObjectPooler.Instance.Get(colideParticle, gameObject.transform.position, new Vector3(0, 0, 0), new Vector2(0.7f, 0.7f));
        Death();
    }
    private IEnumerator DaggerRemainFlow()
    {
        yield return new WaitForSeconds(waitTime);
        StartCoroutine(DaggerFadeFlow());
    }
    private IEnumerator DaggerFadeFlow()
    {
        var total = 1f;
        while (total > 0)
        {
            total -= Time.deltaTime * fadeSpeed;
            sr.color = new Color(1, 1, 1, total);
            yield return null;
        }
        Death();
    }
    public override void OnDeathInit()
    {
        tr.emitting = false;
    }
}
