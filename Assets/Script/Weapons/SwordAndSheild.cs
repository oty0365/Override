using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwordAndSheild: WeaponBase
{

    private const int maxComboCount = 3;
    private bool isBlocking;

    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int BlockHash = Animator.StringToHash("isBlocking");

    private Coroutine comboResetCoroutine;

    [Header("Attack Settings")]
    public float attackSpeed = 1f;
    public float comboInputWindow = 1f;


    private void Start()
    {
        DisableAllHitbox();
    }

    private void Update()
    {
        if (canInput)
        {
            if (Input.GetKeyDown(KeyBindingManager.Instance.keyBindings["Attack1"]) && !isAttacking && !isBlocking)
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

    private void ComboCheck()
    {
        ani.SetInteger(AttackHash, combo);
    }

    public override void SetColider(int index)
    {
        colliders[index].enabled = true;
    }

    public override void EndAnimation()
    {
        isAttacking = false;
    }

    public override void StartCombo()
    {
        isAttacking = true;
    }

    public override void EndCombo()
    {
        combo = 0;
        ComboCheck();
    }


    public override void OnAttack1Pressed()
    {
        StartCombo();
        combo++;
        if (combo > maxComboCount)
        {
            combo = 1;
        }

        ComboCheck();

        if (comboResetCoroutine != null)
            StopCoroutine(comboResetCoroutine);

        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        ani.speed = attackSpeed;
        float animLength = ani.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animLength / attackSpeed);
        ani.speed = 1f;
        EndAnimation();
        if (combo < maxComboCount)
        {
            comboResetCoroutine = StartCoroutine(ComboResetRoutine());
        }
        else
        {
            EndCombo();
        }
    }

    private IEnumerator ComboResetRoutine()
    {
        yield return new WaitForSeconds(comboInputWindow);
        EndCombo();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isBlocking)
        {
            IHitable hitable = other.GetComponent<IHitable>();
            if(hitable != null)
            {
                Vector2 contactPoint = other.ClosestPoint(transform.position);
                ObjectPooler.Instance.Get(colideParticle, contactPoint, new Vector3(0, 0, 0),new Vector2(1.5f,1.5f));
                PlayerCamera.Instance.SetShake(0.5f, 20, 0.03f);
            }

        }    
    }

   /* public override void OnAttack1Pressed()
    {
        combo++;
        if (combo > maxComboCount)
        {
            combo = 1;
        }

        ComboCheck();
    }*/

    public override void OnAttack1Released() { }

    public override void OnAttack2Pressed()
    {
        isBlocking = true;
        isAttacking = false;
        combo = 0;
        PlayerCamera.Instance.SetZoom(3, 4);
        ComboCheck();
        ani.SetBool(BlockHash,true);
    }

    public override void OnAttack2Released()
    {
        PlayerCamera.Instance.SetZoom(4.5f, 4);
        DisableAllHitbox();
        ani.SetBool(BlockHash, false);
        isBlocking = false;
    }

}
