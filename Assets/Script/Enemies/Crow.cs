using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class Crow : Enemy
{
    public APoolingObject colideParticle;

    [Header("¸ðµâ")]
    public PathFinderModule finderModule;
    public RecognitionModule recognitionModule;

    public GameObject attackDir;
    public Vector2 dir;

    public APoolingObject crowBite;
    
    private void Start()
    {
        InitEnemy();
    }
    protected override void Update()
    {
        base.Update();
        if (isStun&&fsm.currentState != fsm.states["Death"])
        {
            fsm.ChangeState(fsm.states["Stun"]);
            isStun = false;
        }
    }
    private IEnumerator AttackCoolDown()
    {
        canAttack = false;
        yield return new WaitForSeconds(2f);
        canAttack = true;
    }

    public void Attacked()
    {
        StartCoroutine(AttackCoolDown());
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }


    public override void Hit(Collider2D collider, float damage, float infiateTime)
    {
        if (fsm.currentState != fsm.states["Stun"]&&fsm.currentState != fsm.states["Death"])
        {
            fsm.ChangeState(fsm.states["Hit"]);
        }
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
        if (CurrentHp <= 0)
        {
            fsm.ChangeState(fsm.states["Death"]);
        }
        Debug.Log(CurrentHp);
    }
    public override void OnBirth()
    {
        InitEnemy();
    }
    public override void OnDeathInit()
    {
    }
    public override void InitEnemy()
    {
        target = PlayerInfo.Instance.gameObject;
        attackDir.SetActive(false);
        base.InitEnemy();
        recognitionModule.Initialize();
        finderModule.Initialize();
        var walk = new CrowWalk();
        var death = new CrowDeath();
        var idel = new CrowIdel();
        var attack = new CrowAttack();
        var stun = new CrowStun();
        var hit = new CrowHit();
        fsm.AddState("Walk", walk,this);
        fsm.AddState("Death", death,this);
        fsm.AddState("Idel", idel,this);
        fsm.AddState("Attack", attack,this);
        fsm.AddState("Stun", stun,this);
        fsm.AddState("Hit", hit, this);
        fsm.ChangeState(fsm.states["Idel"]);
    }
    public void OnDeath()
    {
        DeathDrop();
        StartCoroutine(DeathFlow());
    }
    private IEnumerator DeathFlow()
    {
        yield return new WaitForSeconds(0.8f);
        Death();
    }
}


public class CrowHit : State
{
    float i;
    public override void OnStateStart()
    {
        enemy.ani.Play("CrowIdelMove");
        i = 0;
    }
    public override void OnStateFixedUpdate()
    {

    }
    public override void OnStateUpdate()
    {
        var gb = GetEnemyAs<Crow>();
        i += Time.deltaTime;
        gb.Flip();
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
public class CrowWalk : State
{
    public override void OnStateStart()
    {
        enemy.ani.Play("CrowIdelMove");
        GetEnemyAs<Crow>().finderModule.SetMove();
    }
    public override void OnStateFixedUpdate()
    {

    }

    public override void OnStateUpdate()
    {
        Crow gb = GetEnemyAs<Crow>();
        gb.Flip();
        if (!gb.finderModule.CheckMoveDist()||!gb.recognitionModule.Recognize(enemy.monsterData.recognitionRange))
        {
            gb.fsm.ChangeState(gb.fsm.states["Idel"]);
        }
        if (gb.recognitionModule.Recognize(1f))
        {
            gb.fsm.ChangeState(gb.fsm.states["Attack"]);
        }
    }
    public override void OnStateEnd()
    {
        GetEnemyAs<Crow>().finderModule.Stop();
    }
}
public class CrowDeath : BaseDeath
{
    public override void OnStateStart()
    {
        enemy.staminaPoint.Death();
        enemy.ani.Play("CrowDeath");
        GetEnemyAs<Crow>().OnDeath();
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
public class CrowIdel : BaseIdel
{
    public override void OnStateStart()
    {
        enemy.rb2D.linearVelocity = Vector2.zero;
        enemy.ani.Play("CrowIdelMove");
    }
    public override void OnStateFixedUpdate()
    {

    }
    public override void OnStateUpdate()
    {
        Crow gb = GetEnemyAs<Crow>();
        gb.Flip();
        if (gb.recognitionModule.Recognize(enemy.monsterData.recognitionRange))
        {
            gb.fsm.ChangeState(gb.fsm.states["Walk"]);
        }
    }
    public override void OnStateEnd()
    {

    }
}
public class CrowAttack : BaseAttack
{
    AnimatorStateInfo currAni;
    public override void OnStateStart()
    {
        
        var gb = GetEnemyAs<Crow>();
        if (gb.canAttack)
        {
            if (!gb.isCurrupted)
            {
                gb.Attacked();
            }
            gb.dir = gb.recognitionModule.SolveDirection(gb.target.transform.position, gb.transform.position);

            if (gb.transform.localScale.x < 0)
            {
                gb.dir = new Vector2(-gb.dir.x, gb.dir.y);
            }


            var deg = Mathf.Atan2(gb.dir.y, gb.dir.x) * Mathf.Rad2Deg;

            gb.attackDir.SetActive(true);
            gb.attackDir.transform.localRotation = Quaternion.Euler(0, 0, deg - 90);

            var o = ObjectPooler.Instance.Get(gb.crowBite, gb.transform);
            o.transform.localScale = new Vector2(1.5f, 1.5f);
            o.transform.localPosition = gb.dir.normalized * 1.2f;
            o.transform.localRotation = Quaternion.Euler(0, 0, deg);
            enemy.ani.Play("CrowAttack");
            gb.rb2D.linearVelocity = Vector2.zero;
        }
        else
        {
            gb.fsm.ChangeState(gb.fsm.states["Idel"]);
        }

    }


    public override void OnStateFixedUpdate()
    {
        
    }

    public override void OnStateUpdate()
    {
        currAni = enemy.ani.GetCurrentAnimatorStateInfo(0);
        var gb = GetEnemyAs<Crow>();
        if (currAni.normalizedTime >= 1f)
        {
            gb.fsm.ChangeState(gb.fsm.states["Idel"]);
        }
    }

    public override void OnStateEnd()
    {
        var gb = GetEnemyAs<Crow>();
        gb.attackDir.SetActive(false);
        gb.rb2D.linearVelocity = Vector2.zero;
    }
}
public class CrowStun : BaseStun
{
    float i;
    public override void OnStateStart()
    {
        enemy.ani.Play("CrowStun");
        i = 0;
        enemy.isStunning = true;
        enemy.staminaPoint.sr.color = Color.black;
    }
    public override void OnStateFixedUpdate()
    {

    }
    public override void OnStateUpdate()
    {
        var gb = GetEnemyAs<Crow>();
        i += Time.deltaTime;
        gb.Flip();
        gb.rb2D.linearVelocity = Vector2.zero;
        if (i > 1f)
        {
            gb.fsm.ChangeState(enemy.fsm.states["Idel"]);
        }
    }
    public override void OnStateEnd()
    {
        enemy.isStunning = false;
        enemy.CurrentStamina = enemy.monsterData.maxStamina;
    }
}
