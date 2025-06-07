using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class GreenGoblin : Enemy
{
    public APoolingObject colideParticle;

    [Header("모듈")]
    public PathFinderModule finderModule;
    public RecognitionModule recognitionModule;

    public GameObject attackDir;
    public Vector2 dir;

    public APoolingObject dagger;



    private void Start()
    {
        InitEnemy();
    }
    protected override void Update()
    {
        base.Update();
        if (isStun && fsm.currentState != fsm.states["Death"])
        {
            fsm.ChangeState(fsm.states["Stun"]);
            isStun = false;
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }


    public override void Hit(Collider2D collider, float damage, float infiateTime)
    {
        if (fsm.currentState != fsm.states["Stun"] && fsm.currentState != fsm.states["Death"])
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
    private IEnumerator AttackCoolDown()
    {
        canAttack = false;
        yield return new WaitForSeconds(1.4f);
        canAttack = true;
    }

    public void Attacked()
    {
        StartCoroutine(AttackCoolDown());
    }

    public override void InitEnemy()
    {
        target = PlayerInfo.Instance.gameObject;
        attackDir.SetActive(false);
        base.InitEnemy();
        recognitionModule.Initialize();
        finderModule.Initialize();
        var walk = new GreenGoblinWalk();
        var death = new GreenGoblinDeath();
        var idel = new GreenGoblinIdel();
        var attack = new GreenGoblinAttack();
        var stun = new GreenGoblinStun();
        var hit = new GreenGoblinHit();
        var waitCooldown = new GreenGoblinWaitCooldown(); // 새로운 상태 추가
        fsm.AddState("Walk", walk, this);
        fsm.AddState("Death", death, this);
        fsm.AddState("Idel", idel, this);
        fsm.AddState("Attack", attack, this);
        fsm.AddState("Stun", stun, this);
        fsm.AddState("Hit", hit, this);
        fsm.AddState("WaitCooldown", waitCooldown, this); // 새로운 상태 추가
        fsm.ChangeState(fsm.states["Idel"]);
    }
    public void OnDeath()
    {
        DeathDrop();
        StartCoroutine(DeathFlow());
    }
    private IEnumerator DeathFlow()
    {
        yield return new WaitForSeconds(4f);
        Death();
    }
}


public class GreenGoblinHit : State
{
    float i;
    public override void OnStateStart()
    {
        enemy.ani.Play("GreenGoblinIdel");
        i = 0;
    }
    public override void OnStateFixedUpdate()
    {

    }
    public override void OnStateUpdate()
    {
        var gb = GetEnemyAs<GreenGoblin>();
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
public class GreenGoblinWalk : State
{
    public override void OnStateStart()
    {
        enemy.ani.Play("GreenGoblinWalk");
        GetEnemyAs<GreenGoblin>().finderModule.SetMove();
    }
    public override void OnStateFixedUpdate()
    {

    }

    public override void OnStateUpdate()
    {
        GreenGoblin gb = GetEnemyAs<GreenGoblin>();
        gb.Flip();
        if (!gb.finderModule.CheckMoveDist() || !gb.recognitionModule.Recognize(enemy.monsterData.recognitionRange))
        {
            gb.fsm.ChangeState(gb.fsm.states["Idel"]);
        }
        if (gb.recognitionModule.Recognize(1.4f))
        {
            gb.fsm.ChangeState(gb.fsm.states["Attack"]);
        }
    }
    public override void OnStateEnd()
    {
        GetEnemyAs<GreenGoblin>().finderModule.Stop();
    }
}
public class GreenGoblinDeath : BaseDeath
{
    public override void OnStateStart()
    {
        enemy.staminaPoint.Death();
        enemy.ani.Play("GreenGoblinDeath");
        GetEnemyAs<GreenGoblin>().OnDeath();
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
public class GreenGoblinIdel : BaseIdel
{
    public override void OnStateStart()
    {
        enemy.rb2D.linearVelocity = Vector2.zero;
        enemy.ani.Play("GreenGoblinIdel");
    }
    public override void OnStateFixedUpdate()
    {

    }
    public override void OnStateUpdate()
    {
        GreenGoblin gb = GetEnemyAs<GreenGoblin>();
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

public class GreenGoblinWaitCooldown : State
{
    float waitTime = 0f;
    float maxWaitTime = 1.0f; 

    public override void OnStateStart()
    {
        enemy.ani.Play("GreenGoblinIdel");
        waitTime = 0f;
        enemy.rb2D.linearVelocity = Vector2.zero;
    }

    public override void OnStateFixedUpdate()
    {

    }

    public override void OnStateUpdate()
    {
        var gb = GetEnemyAs<GreenGoblin>();
        waitTime += Time.deltaTime;
        gb.Flip();

        if (!gb.recognitionModule.Recognize(enemy.monsterData.recognitionRange))
        {
            gb.fsm.ChangeState(gb.fsm.states["Idel"]);
            return;
        }

        if (gb.canAttack && gb.recognitionModule.Recognize(1.4f))
        {
            gb.fsm.ChangeState(gb.fsm.states["Attack"]);
            return;
        }

        if (waitTime > maxWaitTime)
        {
            gb.fsm.ChangeState(gb.fsm.states["Walk"]);
        }
    }

    public override void OnStateEnd()
    {

    }
}

public class GreenGoblinAttack : BaseAttack
{
    AnimatorStateInfo currAni;
    bool usedSkill = false;
    float deg = 0f;
    public override void OnStateStart()
    {
        var gb = GetEnemyAs<GreenGoblin>();
        if (gb.canAttack)
        {
            usedSkill = false;
            if (!gb.isCurrupted)
            {
                gb.Attacked();
            }
            gb.rb2D.linearVelocity = Vector2.zero;
            gb.attackDir.SetActive(true);
            enemy.ani.Play("GreenGoblinAttack");
        }
        else
        {
            gb.fsm.ChangeState(gb.fsm.states["WaitCooldown"]);
        }

    }
    public override void OnStateFixedUpdate()
    {

    }
    public override void OnStateUpdate()
    {

        var gb = GetEnemyAs<GreenGoblin>();
        currAni = gb.ani.GetCurrentAnimatorStateInfo(0);
        if (currAni.normalizedTime >= 1.0f)
        {
            gb.fsm.ChangeState(enemy.fsm.states["Idel"]);
        }
        else if (currAni.normalizedTime >= 0.5f)
        {
            if (!usedSkill)
            {
                usedSkill = true;
                var o = ObjectPooler.Instance.Get(gb.dagger, gb.transform);
                o.transform.localScale = new Vector2(3.5f, 3.5f);
                o.transform.localPosition = gb.dir.normalized * 1.2f;
                o.transform.localRotation = Quaternion.Euler(0, 0, deg);
            }

        }
        else
        {
            gb.dir = gb.recognitionModule.SolveDirection(gb.target.transform.position, gb.transform.position);
            if (gb.transform.localScale.x < 0)
            {
                gb.dir = new Vector2(-gb.dir.x, gb.dir.y);
            }
            deg = Mathf.Atan2(gb.dir.y, gb.dir.x) * Mathf.Rad2Deg;
            gb.attackDir.transform.localRotation = Quaternion.Euler(0, 0, deg - 90);
        }

    }

    public override void OnStateEnd()
    {
        var gb = GetEnemyAs<GreenGoblin>();
        gb.attackDir.SetActive(false);
        gb.rb2D.linearVelocity = Vector2.zero;
    }
}
public class GreenGoblinStun : BaseStun
{
    float i;
    public override void OnStateStart()
    {
        enemy.ani.Play("GreenGoblinStun");
        i = 0;
        enemy.isStunning = true;
        enemy.staminaPoint.sr.color = Color.black;
    }
    public override void OnStateFixedUpdate()
    {

    }
    public override void OnStateUpdate()
    {
        var gb = GetEnemyAs<GreenGoblin>();
        i += Time.deltaTime;
        gb.Flip();
        gb.rb2D.linearVelocity = Vector2.zero;
        if (i > 2f)
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