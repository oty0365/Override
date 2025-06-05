using NUnit.Framework.Constraints;
using System.Collections;
using UnityEngine;

public class ThrownDagger: AAttack
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private float fadeSpeed;
    [SerializeField] private float shootSpeed;
    //[SerializeField] private LineRenderer lr;
    [SerializeField] APoolingObject colideParticle;

    private PlayerInfo _playerInfo;
    public override void OnBirth()
    {
        _playerInfo = PlayerInfo.Instance;
        //lr.SetPosition(0, gameObject.transform.position);
        //lr.SetPosition(1, _playerInfo.gameObject.transform.position);
        sr.color = new Color(1, 1, 1, 1);
        rb.linearVelocity = transform.right.normalized * shootSpeed;
        StartCoroutine(DaggerRemainFlow());
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        rb.linearVelocity = Vector2.zero;
        //Vector2 contactPoint = other.ClosestPoint(transform.position);
        ObjectPooler.Instance.Get(colideParticle, gameObject.transform.position, new Vector3(0, 0, 0), new Vector2(0.65f, 0.65f));
    }

    private IEnumerator DaggerRemainFlow()
    {
        yield return new WaitForSeconds(2f);
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

    }
}
