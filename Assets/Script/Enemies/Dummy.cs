using UnityEngine;

public class Dummy : Enemy
{
    public APoolingObject colideParticle;
    private bool isInitialized;

    private void Start()
    {
        InitEnemy();
    }
    public override void Hit(float damage)
    {
        base.Hit(damage);
        PlayerCamera.Instance.SetShake(0.2f, 7.5f, 0.13f);
        ani.Play("DumyHit");
        ObjectPooler.Instance.Get(colideParticle, gameObject.transform.position, new Vector3(0, 0, 0), new Vector2(1f, 1f));
        Vector2 center = gameObject.transform.position;
        float radius = 3.5f;
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        float randomDistance = Random.Range(1, radius);
        Vector2 spawnPos = center + randomDirection * randomDistance;
        var o = ObjectPooler.Instance.Get(ObjGenerator.Instance.generateDict["Code"], gameObject.transform.position, new Vector3(0, 0, 0));
        o.GetComponent<Code>().target = spawnPos;
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
