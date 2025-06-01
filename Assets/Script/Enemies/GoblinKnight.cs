using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

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


    public override void Hit(Collider2D collider, float damage, float infiateTime)
    {
        fsm.ChangeState(fsm.states["Hit"]);
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
        var hit = new GoblinKnightHit();
        var aim = new GoblinKnightAim();
        fsm.AddState("Walk", walk,this);
        fsm.AddState("Death", death,this);
        fsm.AddState("Idel", idel,this);
        fsm.AddState("Attack", attack,this);
        fsm.AddState("Stun", stun,this);
        fsm.AddState("Hit", hit, this);
        fsm.AddState("Aim", aim, this);
        fsm.ChangeState(fsm.states["Idel"]);
    }
}

public class GoblinKnightHit : State
{
    float i;
    public override void OnStateStart()
    {
        enemy.ani.Play("GoblinKnightIdel");
        i = 0;
    }
    public override void OnStateFixedUpdate()
    {

    }
    public override void OnStateUpdate()
    {
        var gb = GetEnemyAs<GoblinKnight>();
        i += Time.deltaTime;
        gb.rb2D.linearVelocity = Vector2.zero;
        if (i > 0.2f)
        {
            gb.fsm.ChangeState(enemy.fsm.states["Idel"]);
        }
    }
    public override void OnStateEnd()
    {
        
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
        if (!gb.finderModule.CheckMoveDist()||!gb.recognitionModule.Recognize(enemy.monsterData.recognitionRange))
        {
            gb.fsm.ChangeState(gb.fsm.states["Idel"]);
        }
        if (gb.recognitionModule.Recognize(2f))
        {
            gb.fsm.ChangeState(gb.fsm.states["Aim"]);
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
public class GoblinKnightAim : BaseAttack
{
    float i = 0f;
    public override void OnStateStart()
    {
        enemy.ani.Play("GoblinKnightAttack");
        i = 0f;
    }
    public override void OnStateFixedUpdate()
    {

    }
    public override void OnStateUpdate()
    {
        var gb = GetEnemyAs<GoblinKnight>();
        i += Time.deltaTime;
        gb.rb2D.linearVelocity = Vector2.zero;
        if (i > 1f)
        {
            gb.fsm.ChangeState(enemy.fsm.states["Attack"]);
        }
    }
    public override void OnStateEnd()
    {

    }
}
public class GoblinKnightAttack : BaseAttack
{
    float i = 0f;
    Vector2 dir;
    bool isUsingRaycast = false;
    Vector2 targetPosition;
    float moveSpeed = 15f;

    public override void OnStateStart()
    {
        var gb = GetEnemyAs<GoblinKnight>();
        enemy.ani.Play("GoblinKnightFire");
        i = 0f;
        dir = gb.recognitionModule.SolveDirection(gb.target.transform.position, gb.transform.position);


        float attackDistance = 100f;
        RaycastHit2D hit = Physics2D.Raycast(gb.transform.position, dir, attackDistance, LayerMask.GetMask("Wall"));
        Debug.Log(hit.collider);

        if (hit.collider != null)
        {

            isUsingRaycast = true;
            targetPosition = hit.point - dir * 1f; 
            gb.rb2D.linearVelocity = Vector2.zero; 
        }
        else
        {
            isUsingRaycast = false;
            gb.rb2D.linearVelocity = dir * moveSpeed;
        }
    }

    public override void OnStateFixedUpdate()
    {
        if (isUsingRaycast)
        {
            var gb = GetEnemyAs<GoblinKnight>();
            Vector2 currentPos = gb.transform.position;

            Vector2 newPos = Vector2.MoveTowards(currentPos, targetPosition, moveSpeed * Time.fixedDeltaTime);

            gb.rb2D.MovePosition(newPos);

            if (Vector2.Distance(currentPos, targetPosition) <0.1f)
            {
                gb.rb2D.linearVelocity = Vector2.zero;
                gb.fsm.ChangeState(enemy.fsm.states["Idel"]);
            }
        }
    }

    public override void OnStateUpdate()
    {
        var gb = GetEnemyAs<GoblinKnight>();
        i += Time.deltaTime;
        if (i > 0.21f)
        {
            gb.fsm.ChangeState(enemy.fsm.states["Idel"]);
        }
    }

    public override void OnStateEnd()
    {
        var gb = GetEnemyAs<GoblinKnight>();
        gb.rb2D.linearVelocity = Vector2.zero;
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
