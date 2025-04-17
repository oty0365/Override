using System;
using UnityEngine;

public interface IHitable 
{
    public void OnHit();
}

public abstract class WeaponBase : MonoBehaviour
{
    public Animator ani;
    public Collider2D[] colliders;
    public bool isAttacking;
    public int combo;
    public APoolingObject colideParticle;
    public float range;

    public void DisableAllHitbox()
    {
        foreach( var i in colliders)
        {
            if (i.enabled == true)
            {
                i.enabled = false;
            }
        }
    }

    public abstract void OnAttack1Pressed();
    public abstract void OnAttack1Released();
    public abstract void OnAttack2Pressed();
    public abstract void OnAttack2Released();

    public abstract void SetColider(int index);
    public abstract void EndAnimation();
    public abstract void StartCombo();
    public abstract void EndCombo();
}
