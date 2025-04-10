using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    public Animator ani;
    public Collider[] colliders;
    public bool isAttacking;
    public int combo;
    public float comboDuration;
    public Coroutine previousCombo;

    public abstract void OnAttack1Pressed();
    public abstract void OnAttack1Released();
    public abstract void OnAttack2Pressed();
    public abstract void OnAttack2Released();

    public abstract void SetColider(int mode, int index);
    public abstract void EndAnimation();
}
