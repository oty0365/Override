using UnityEngine;

public class Spear : WeaponBase
{
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int ReturnHash = Animator.StringToHash("Return");

    private void OnTriggerEnter2D(Collider2D other)
    {
        IHitable hitable = other.GetComponent<IHitable>();
        if (hitable != null)
        {
            Vector2 contactPoint = other.ClosestPoint(transform.position);
            ObjectPooler.Instance.Get(colideParticle, contactPoint, new Vector3(0, 0, 0));
            PlayerCamera.Instance.SetShake(0.4f, 30, 0.02f);
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

    }
    public override void OnAttack2Released()
    {

    }
    public override void OnAttack1Released()
    {

    }
    public override void EndAnimation()
    {

    }
    void Start()
    {
        DisableAllHitbox();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyBindingManager.Instance.keyBindings["Attack1"]) && !isAttacking)
        {
            OnAttack1Pressed();
        }

    }
}
