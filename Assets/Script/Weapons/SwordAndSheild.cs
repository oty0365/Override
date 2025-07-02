using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwordAndSheild : WeaponBase
{
    private const int maxComboCount = 3;
    public static bool isBlocking;
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int BlockHash = Animator.StringToHash("isBlocking");
    private Coroutine comboResetCoroutine;
    private Coroutine blockingCoroutine;

    [Header("Attack Settings")]
    public float attackSpeed = 1f;
    public float comboInputWindow = 1f;
    public Attack attack;

    [Header("Block Settings")]
    public float staminaDrainRate = 10f; // 초당 스테미나 소모량

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
            var a = other.GetComponent<Enemy>();
            if (a != null)
            {
                a.Hit(attack.attackCollider, attack.damage, attack.infinateTime);
            }
            if (hitable != null)
            {
                Vector2 contactPoint = other.ClosestPoint(transform.position);
                hitable.OnHit();
            }
        }
    }

    public override void OnAttack1Released() { }

    public override void OnAttack2Pressed()
    {
        if (PlayerInfo.Instance.PlayerCurStamina <= 0)
            return;

        isBlocking = true;
        isAttacking = false;
        combo = 0;
        PlayerCamera.Instance.SetZoom(3, 4);
        ComboCheck();
        ani.SetBool(BlockHash, true);

        if (blockingCoroutine != null)
            StopCoroutine(blockingCoroutine);
        blockingCoroutine = StartCoroutine(BlockingStaminaDrain());
    }

    public override void OnAttack2Released()
    {
        ReleaseBlock();
    }

    private void ReleaseBlock()
    {
        if (!isBlocking) return;

        PlayerCamera.Instance.SetZoom(4.5f, 4);
        DisableAllHitbox();
        ani.SetBool(BlockHash, false);
        isBlocking = false;

        if (blockingCoroutine != null)
        {
            StopCoroutine(blockingCoroutine);
            blockingCoroutine = null;
        }
    }

    private void ForceReleaseBlock()
    {
        ReleaseBlock();
        Debug.Log("스테미나 부족으로 가드가 해제되었습니다!");
    }

    private IEnumerator BlockingStaminaDrain()
    {
        while (isBlocking && PlayerInfo.Instance.PlayerCurStamina > 0)
        {
            yield return new WaitForSeconds(0.1f);
            PlayerInfo.Instance.PlayerCurStamina -= staminaDrainRate * 0.1f;

            if (PlayerInfo.Instance.PlayerCurStamina <= 0)
            {
                ForceReleaseBlock();
                break;
            }
        }
    }
}