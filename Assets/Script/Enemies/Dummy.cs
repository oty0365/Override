using UnityEngine;

public class Dummy : Enemy,IHitable
{
    public APoolingObject colideParticle;
    private bool isInitialized;

    private void Start()
    {
        if (!isInitialized)
        {
            InitEnemy();
        }
    }
    public void OnHit()
    {
        ani.Play("DumyHit");
        ObjectPooler.Instance.Get(colideParticle, gameObject.transform.position, new Vector3(0, 0, 0), new Vector2(1f, 1f));
        Hit();
    }
    public override void Hit()
    {
        base.Hit();
        PlayerCamera.Instance.SetShake(0.3f, 10, 0.2f);
    }

    public override void OnBirth()
    {
        InitEnemy();
        isInitialized = true;
    }
    public override void OnDeathInit()
    {
        
    }

}
