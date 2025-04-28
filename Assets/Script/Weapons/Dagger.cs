using System.Collections;
using UnityEngine;

public class Dagger : WeaponBase
{
    [SerializeField] private APoolingObject daggerBullet;
    public float daggerThrowCooldown;

    private GameObject _curdagger;
    private bool _isThrowing;
    private bool _canThrow;

    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int ReturnHash = Animator.StringToHash("Return");

    private void OnTriggerEnter2D(Collider2D other)
    {
        IHitable hitable = other.GetComponent<IHitable>();
        if (hitable != null)
        {
            Vector2 contactPoint = other.ClosestPoint(transform.position);
            ObjectPooler.Instance.Get(colideParticle, contactPoint, Vector3.zero, Vector2.one);
            PlayerCamera.Instance.SetShake(0.4f, 20, 0.02f);
            Debug.Log("검과 충돌");
        }
    }

    public override void OnAttack1Pressed()
    {
        isAttacking = true;
        ani.SetTrigger(AttackHash);
    }

    public override void SetColider(int index)
    {
        colliders[index].enabled = true;
    }

    public override void StartCombo()
    {
        isAttacking = false;
    }

    public override void EndCombo() { }

    public override void OnAttack2Pressed()
    {
        if (_curdagger != null && _curdagger.activeSelf)
        {
            Vector2 targetPos = _curdagger.transform.position;
            Vector2 currentPos = PlayerMove.Instance.transform.position;
            Vector2 dir = (targetPos - currentPos).normalized;

            RaycastHit2D hit = Physics2D.Raycast(currentPos, dir, Vector2.Distance(currentPos, targetPos), LayerMask.GetMask("Wall"));
            if (hit.collider == null)
            {
                PlayerMove.Instance.transform.position = targetPos;
            }
            else
            {
                PlayerMove.Instance.transform.position = hit.point - dir * 0.1f;
            }

            ObjectPooler.Instance.Return(_curdagger.GetComponent<APoolingObject>());
            _isThrowing = false;
        }

        if ((_curdagger == null || !_curdagger.activeSelf) && !_isThrowing && _canThrow)
        {
            _isThrowing = true;
            _canThrow = false;

            var playerMove = PlayerMove.Instance;
            _curdagger = ObjectPooler.Instance.Get(
                daggerBullet,
                playerMove.transform.position,
                new Vector3(0, 0, WeaponCore.Instance.rotaion)
            );

            StartCoroutine(DaggerCooldown());
        }
    }

    private IEnumerator DaggerCooldown()
    {
        yield return new WaitForSeconds(daggerThrowCooldown);
        _canThrow = true;
    }

    public override void OnAttack2Released() { }

    public override void OnAttack1Released() { }

    public override void EndAnimation() { }

    void Start()
    {
        _canThrow = true;
        _isThrowing = false;
        DisableAllHitbox();
    }

    void Update()
    {
        if (_curdagger != null && !_curdagger.activeSelf && _isThrowing)
        {
            _isThrowing = false;
        }
        if (canInput)
        {
            if (Input.GetKeyDown(KeyBindingManager.Instance.keyBindings["Attack1"]) && !isAttacking)
            {
                OnAttack1Pressed();
            }

            if (Input.GetKeyDown(KeyBindingManager.Instance.keyBindings["Attack2"]))
            {
                OnAttack2Pressed();
            }

            if (Input.GetKeyUp(KeyBindingManager.Instance.keyBindings["Attack2"]))
            {
                OnAttack2Released();
            }
        }


    }
}
