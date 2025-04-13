using System.Collections;
using UnityEngine;

public class SwordAndSheild: WeaponBase
{

    private const int maxComboCount = 3;
    private bool isBlocking;

    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int BlockHash = Animator.StringToHash("isBlocking");
 

    private void Start()
    {
        DisableAllHitbox();
    }

    private void Update()
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

    public override void OnAttack1Pressed()
    {
        combo++;
        if (combo > maxComboCount)
        {
            combo = 1;
        }

        ComboCheck();
    }

    private void ComboCheck()
    {
        ani.SetInteger(AttackHash, combo);
    }
    public override void SetColider(int index)
    {
        colliders[index].enabled = true;
    }

    public override void OnAttack1Released() { }

    public override void OnAttack2Pressed()
    {
        isBlocking = true;
        isAttacking = false;
        combo = 0;
        ComboCheck();
        ani.SetBool(BlockHash,true);
    }

    public override void OnAttack2Released()
    {
        ani.SetBool(BlockHash, false);
        isBlocking = false;
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
}
