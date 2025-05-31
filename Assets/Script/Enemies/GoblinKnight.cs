using UnityEngine;

public class GoblinKnight : Enemy
{
    public APoolingObject colideParticle;

    [Header("¸ðµâ")]
    public PathFinderModule finderModule;
    public RecognitionModule recognitionModule;
    
    private void Start()
    {
        InitEnemy();
    }

    private void Update()
    {
        StateUpdate();
    }
    private void FixedUpdate()
    {
        StateFixedUpdate();
    }
    public override void Hit(Collider2D collider, float damage, float infiateTime)
    {
        base.Hit(collider, damage, infiateTime);
        PlayerCamera.Instance.SetShake(0.2f, 7.5f, 0.13f);
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
    }
    public override void OnDeathInit()
    {
    }
    public override void InitEnemy()
    {
        base.InitEnemy();
        recognitionModule.Initialize();
        finderModule.Initialize();
        var walk = new GoblinKnightWalk();
        var death = new GoblinKnightDeath();
        var idel = new GoblinKnightIdel();
        var attack = new GoblinKnightAttack();
        var stun = new GoblinKnightStun();
        fsm.AddState("Walk", walk,this);
        fsm.AddState("Death", death,this);
        fsm.AddState("Idel", idel,this);
        fsm.AddState("Attack", attack,this);
        fsm.AddState("Stun", stun,this);
        fsm.ChangeState(fsm.states["Idel"]);
    }
}


public class GoblinKnightWalk : State
{
    public override void OnStateStart()
    {
        enemy.ani.Play("GoblinKnightWalk");
        GetEnemyAs<GoblinKnight>().finderModule.SetMove();
    }
    public override void OnStateFixedUpdate()
    {

    }

    public override void OnStateUpdate()
    {
        GoblinKnight gb = GetEnemyAs<GoblinKnight>();
        gb.finderModule.CheckMoveDist();
        if (!gb.recognitionModule.Recognize(enemy.monsterData.recognitionRange))
        {
            gb.fsm.ChangeState(gb.fsm.states["Idel"]);
        }
    }
    public override void OnStateEnd()
    {
        GetEnemyAs<GoblinKnight>().finderModule.Stop();
    }
}
public class GoblinKnightDeath : BaseDeath
{
    public override void OnStateStart()
    {

    }
    public override void OnStateFixedUpdate()
    {

    }
    public override void OnStateUpdate()
    {

    }
    public override void OnStateEnd()
    {

    }
}
public class GoblinKnightIdel : BaseIdel
{
    public override void OnStateStart()
    {
        enemy.rb2D.linearVelocity = Vector2.zero;
        enemy.ani.Play("GoblinKnightIdel");
    }
    public override void OnStateFixedUpdate()
    {

    }
    public override void OnStateUpdate()
    {
        GoblinKnight gb = GetEnemyAs<GoblinKnight>();
        if (gb.recognitionModule.Recognize(enemy.monsterData.recognitionRange))
        {
            gb.fsm.ChangeState(gb.fsm.states["Walk"]);
        }
    }
    public override void OnStateEnd()
    {

    }
}
public class GoblinKnightAttack : BaseAttack
{
    public override void OnStateStart()
    {

    }
    public override void OnStateFixedUpdate()
    {

    }
    public override void OnStateUpdate()
    {

    }
    public override void OnStateEnd()
    {

    }
}
public class GoblinKnightStun : BaseStun
{
    public override void OnStateStart()
    {

    }
    public override void OnStateFixedUpdate()
    {

    }
    public override void OnStateUpdate()
    {

    }
    public override void OnStateEnd()
    {

    }
}
