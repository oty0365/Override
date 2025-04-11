using System.Collections;
using UnityEngine;

public class SwordAndSheild: WeaponBase
{

    private const int maxComboCount = 3;
    private bool isBlocking;

    private static readonly int Attack1Hash = Animator.StringToHash("Attack1");
    private static readonly int Attack2Hash = Animator.StringToHash("Attack2");
    private static readonly int Attack3Hash = Animator.StringToHash("Attack3");
    private static readonly int ReturnHash = Animator.StringToHash("Return");
   

    private Coroutine comboCoroutine;
    private Coroutine attackResetCoroutine;

    private void Update()
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
            OnAttack1Released();
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
        switch (combo)
        {
            case 1:
                ani.SetTrigger(Attack1Hash);
                break;
            case 2:
                ani.SetTrigger(Attack2Hash);
                break;
            case 3:
                ani.SetTrigger(Attack3Hash);
                break;
        }
    }

    public override void OnAttack1Released() { }

    public override void OnAttack2Pressed()
    {
    }

    public override void OnAttack2Released()
    {
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
        ani.SetTrigger(ReturnHash);
    }

    public override void SetColider(int mode, int index)
    {
    }
}
