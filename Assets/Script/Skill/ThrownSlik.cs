using NUnit.Framework.Constraints;
using System.Collections;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;

public class ThrownSlik: AAttack
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private float fadeSpeed;
    [SerializeField] private float shootSpeed;
    [SerializeField] private LineRenderer lr;
    [SerializeField] APoolingObject colideParticle;
    public float remainTime;
    private Coroutine _slikReturnFlow;
    public Vector2 target;

    private PlayerInfo _playerInfo;
    public override void OnBirth()
    {
        _playerInfo = PlayerInfo.Instance;
        lr.SetPosition(0, gameObject.transform.position);
        lr.SetPosition(1, _playerInfo.gameObject.transform.position);
        sr.color = new Color(1, 1, 1, 1);
        rb.linearVelocity = transform.up.normalized * shootSpeed;
        _slikReturnFlow=StartCoroutine(SlikReturnFlow());
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        rb.linearVelocity = Vector2.zero;
        ObjectPooler.Instance.Get(colideParticle, gameObject.transform.position, new Vector3(0, 0, 0), new Vector2(0.65f, 0.65f));
        if (Vector2.Distance(collision.ClosestPoint(transform.position), target) > 1.5f)
        {
            target = collision.ClosestPoint(transform.position);
        }
        StopCoroutine(_slikReturnFlow);
        StartCoroutine(GoToSlik());
    }
    private IEnumerator SlikReturnFlow()
    {
        yield return new WaitForSeconds(remainTime);
        StartCoroutine(ReturnFlow());
    }
    private IEnumerator GoToSlik()
    {
        while (Vector2.Distance(_playerInfo.transform.position, target) > 0.1f)
        {
            _playerInfo.transform.position = Vector2.MoveTowards(_playerInfo.transform.position, target, Time.deltaTime * 22f);
            yield return null;
        }
        Death();
    }
    private IEnumerator ReturnFlow()
    {
        rb.linearVelocity = Vector2.zero;
        while (Vector2.Distance(_playerInfo.transform.position, gameObject.transform.position) > 0.1f)
        {
            gameObject.transform.position = Vector2.MoveTowards(gameObject.transform.position,_playerInfo.transform.position,Time.deltaTime * 22f);
            yield return null;
        }
        Death();
    }
    private void Update()
    {
        lr.SetPosition(0, gameObject.transform.position);
        lr.SetPosition(1, _playerInfo.gameObject.transform.position);
    }

    public override void OnDeathInit()
    {

    }
}
