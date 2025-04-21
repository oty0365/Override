using UnityEngine;

public class Dagger: WeaponBase
{
    [SerializeField] private APoolingObject daggerBullet;
    private GameObject _curdagger;
    private bool isThrowing;
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int ReturnHash = Animator.StringToHash("Return");

    private void OnTriggerEnter2D(Collider2D other)
    {
        IHitable hitable = other.GetComponent<IHitable>();
        if (hitable != null)
        {
            Vector2 contactPoint = other.ClosestPoint(transform.position);
            ObjectPooler.Instance.Get(colideParticle, contactPoint, new Vector3(0, 0, 0),new Vector2(1,1));
            PlayerCamera.Instance.SetShake(0.4f, 20, 0.02f);
            Debug.Log("검과충돌");
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
    public override void EndCombo()
    {
    }
    public override void OnAttack2Pressed()
    {
        if (_curdagger != null && _curdagger.activeSelf)
        {
            var parentObj = PlayerMove.Instance.gameObject;
            parentObj.transform.position = _curdagger.transform.position;
            ObjectPooler.Instance.Return(_curdagger.GetComponent<APoolingObject>());
            isThrowing = false;
        }
        else if (_curdagger==null||!_curdagger.activeSelf&&!isThrowing)
        {
            isThrowing = true;
            var playerMove = PlayerMove.Instance;
            _curdagger = ObjectPooler.Instance.Get(daggerBullet, playerMove.transform.position, new Vector3(0, 0, WeaponCore.Instance.rotaion));
        }
       
    }
    public override void OnAttack2Released()
    {/*
        var playerMove = PlayerMove.Instance;
        runWay.SetActive(false);
        PlayerCamera.Instance.SetZoom(4.5f, 4);
        playerMove.canInput = true;
        playerMove.PlayerBehave = PlayerBehave.Idel;
        */
    }
    public override void OnAttack1Released()
    {

    }
    public override void EndAnimation()
    {

    }
    void Start()
    {
        isThrowing = false;
        DisableAllHitbox();
    }

    void Update()
    {
        if (_curdagger != null && !_curdagger.activeSelf&&isThrowing)
        {
            isThrowing = false;
        }

        if (Input.GetKeyDown(KeyBindingManager.Instance.keyBindings["Attack1"]) && !isAttacking)
        {
            OnAttack1Pressed();
        }
        if (Input.GetKeyDown(KeyBindingManager.Instance.keyBindings["Attack2"]) && !isAttacking)
        {
            OnAttack2Pressed();
        }
        if (Input.GetKeyUp(KeyBindingManager.Instance.keyBindings["Attack2"]))
        {
            OnAttack2Released();
        }
    }
}
