using System.Collections;
using UnityEngine;

public class MagicStaff : WeaponBase
{
    [SerializeField] private APoolingObject[] bullets;
    public float castCooldown = 0.2f;
    private GameObject _curdagger;
    private bool _canThrow;
    private bool _isOnCooldown = false;

    public override void OnAttack1Pressed()
    {
        if (_isOnCooldown) return;

        var playerMove = PlayerMove.Instance;
        _curdagger = ObjectPooler.Instance.Get(
            bullets[0],
            playerMove.transform.position,
            new Vector3(0, 0, WeaponCore.Instance.rotaion)
        );

        StartCoroutine(AttackDelay());
    }

    private IEnumerator AttackDelay()
    {
        _isOnCooldown = true;
        yield return new WaitForSeconds(castCooldown);
        _isOnCooldown = false;
    }

    public override void SetColider(int index)
    {
        //colliders[index].enabled = true;
    }

    public override void StartCombo()
    {
        isAttacking = false;
    }

    public override void EndCombo() { }

    public override void OnAttack2Pressed()
    {
    }

    public override void OnAttack2Released() { }

    public override void OnAttack1Released() { }

    public override void EndAnimation() { }

    void Start()
    {
        _canThrow = true;
        //DisableAllHitbox();
    }

    void Update()
    {
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