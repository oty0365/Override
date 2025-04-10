using System.Collections;
using UnityEngine;

public class SwordAndSheild : WeaponBase
{

    private void Update()
    {
        if (Input.GetKeyDown(KeyBindingManager.Instance.keyBindings["Attack1"])&&!isAttacking)
        {
            isAttacking = true;
            OnAttack1Pressed();
        }
    }
    public override void OnAttack1Pressed()
    {
        combo++;
        if (combo > 3)
        {
            combo = 1;
        }
        if(previousCombo != null)
        {
            StopCoroutine(previousCombo);
        }
        previousCombo = StartCoroutine(AttackFlow());
    }
    private IEnumerator AttackFlow()
    {
        switch (combo)
        {
            case 1:
                ani.SetTrigger("Attack1");
                break;
            case 2:
                ani.SetTrigger("Attack2");
                break;
            case 3:
                ani.SetTrigger("Attack3");
                break;
        }
        yield return new WaitForSeconds(comboDuration);
        combo = 0;
    }

    public override void OnAttack1Released()
    {

    }
    public override void OnAttack2Pressed()
    {

    }

    public override void OnAttack2Released()
    {

    }

    public override void EndAnimation()
    {
        ani.SetTrigger("Return");
        isAttacking = false;
        
    }
    public override void SetColider(int mode, int index)
    {
        
    }
}
