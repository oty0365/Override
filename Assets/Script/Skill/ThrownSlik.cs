using NUnit.Framework.Constraints;
using System.Collections;
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
    private Rigidbody2D _playerRb;
    private PlayerInfo _playerInfo;
    public override void OnBirth()
    {
        _playerInfo = PlayerInfo.Instance;
        _playerRb = _playerInfo.gameObject.GetComponent<Rigidbody2D>();
        lr.SetPosition(0, gameObject.transform.position);
        lr.SetPosition(1, _playerInfo.gameObject.transform.position);
        sr.color = new Color(1, 1, 1, 1);
        rb.linearVelocity = transform.up.normalized * shootSpeed;
        _slikReturnFlow=StartCoroutine(SlikReturnFlow());
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        rb.linearVelocity = Vector2.zero;
        
        Vector2 contactPoint = collision.ClosestPoint(transform.position);
        
        Vector2 dir = rb.linearVelocity.sqrMagnitude > 0.01f ? rb.linearVelocity : (Vector2)transform.up;
        
        float offset = 0.2f; 
        Vector2 correctedPos = contactPoint - dir.normalized * offset;

        transform.position = correctedPos;

        var a = ObjectPooler.Instance.Get(
            colideParticle,
            transform.position,
            Vector3.zero,
            new Vector2(0.65f, 0.65f)
        );

        if (Vector2.Distance(contactPoint, target) > 1.5f)
        {
            target = contactPoint;
        }

        StopCoroutine(_slikReturnFlow);
        if (a.activeSelf)
        {
            StartCoroutine(GoToSlik());
        }
    }

    private IEnumerator SlikReturnFlow()
    {
        yield return new WaitForSeconds(remainTime);
        StartCoroutine(ReturnFlow());
    }
    private IEnumerator GoToSlik()
    {
        Vector2 prevPos = _playerRb.position; 
        int stuckFrameCount = 0;              
        int maxStuckFrames = 5;                

        while (Vector2.Distance(_playerRb.position, target) > 0.4f)
        {
            Vector2 newPos = Vector2.MoveTowards(_playerRb.position, target, Time.fixedDeltaTime * 30f);
            _playerRb.MovePosition(newPos);

            yield return new WaitForFixedUpdate();
            if (Vector2.Distance(_playerRb.position, prevPos) < 0.001f)
            {
                stuckFrameCount++;
                if (stuckFrameCount >= maxStuckFrames)
                {
                    Death();
                    yield break;
                }
            }
            else
            {
                stuckFrameCount = 0;
            }

            prevPos = _playerRb.position;
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
