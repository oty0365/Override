using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class SkeletonArch : Enemy
{
    public APoolingObject colideParticle;

    [Header("¸ðµâ")]
    public PathFinderModule finderModule;
    public RecognitionModule recognitionModule;

    public GameObject attackDir;
    public Vector2 dir;

    public APoolingObject arrow;

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
        yield return new WaitForSeconds(2.1f);
        canAttack = true;
    }

    public void Attacked()
    {
        StartCoroutine(AttackCoolDown());
    }

    /*public void Flip()
    {
        if (target != null)
        {
            Vector3 scale = transform.localScale;

            if (target.transform.position.x < gameObject.transform.position.x)
            {
                scale.x = -Mathf.Abs(scale.x);
            }
            else
            {
                scale.x = Mathf.Abs(scale.x);
            }

            transform.localScale = scale;
        }
    }*/

    public override void InitEnemy()
    {
        target = PlayerInfo.Instance.gameObject;
        attackDir.SetActive(false);
        base.InitEnemy();
        recognitionModule.Initialize();
        finderModule.Initialize();
        var walk = new SkeletonArchWalk();
        var death = new SkeletonArchDeath();
        var idel = new SkeletonArchIdel();
        var attack = new SkeletonArchAttack();
        var stun = new SkeletonArchStun();
        var hit = new SkeletonArchHit();
        var waitCooldown = new SkeletonArchWaitCooldown();
        fsm.AddState("Walk", walk, this);
        fsm.AddState("Death", death, this);
        fsm.AddState("Idel", idel, this);
        fsm.AddState("Attack", attack, this);
        fsm.AddState("Stun", stun, this);
        fsm.AddState("Hit", hit, this);
        fsm.AddState("WaitCooldown", waitCooldown, this);
        fsm.ChangeState(fsm.states["Idel"]);
    }

    public void OnDeath()
    {
        StartCoroutine(DeathFlow());
    }

    private IEnumerator DeathFlow()
    {
        yield return new WaitForSeconds(4f);
        Death();
    }
}

public class SkeletonArchHit : State
{
    float i;
    public override void OnStateStart()
    {
        enemy.ani.Play("SkeletonArchIdel");
        i = 0;
    }
    public override void OnStateFixedUpdate()
    {

    }
    public override void OnStateUpdate()
    {
        var sa = GetEnemyAs<SkeletonArch>();
        i += Time.deltaTime;
        sa.Flip();
        sa.rb2D.linearVelocity = Vector2.zero;
        if (i > 0.2f)
        {
            sa.fsm.ChangeState(enemy.fsm.states["Idel"]);
        }
    }
    public override void OnStateEnd()
    {

    }
}

public class SkeletonArchWalk : State
{
    public override void OnStateStart()
    {
        enemy.ani.Play("SkeletonArchWalk");
        GetEnemyAs<SkeletonArch>().finderModule.SetMove();
    }
    public override void OnStateFixedUpdate()
    {

    }

    public override void OnStateUpdate()
    {
        SkeletonArch sa = GetEnemyAs<SkeletonArch>();
        sa.Flip();
        if (!sa.finderModule.CheckMoveDist() || !sa.recognitionModule.Recognize(enemy.monsterData.recognitionRange))
        {
            sa.fsm.ChangeState(sa.fsm.states["Idel"]);
        }
        if (sa.recognitionModule.Recognize(10f))
        {
            sa.fsm.ChangeState(sa.fsm.states["Attack"]);
        }
    }
    public override void OnStateEnd()
    {
        GetEnemyAs<SkeletonArch>().finderModule.Stop();
    }
}

public class SkeletonArchDeath : BaseDeath
{
    public override void OnStateStart()
    {
        enemy.ani.Play("SkeletonArchDeath");
        GetEnemyAs<SkeletonArch>().OnDeath();
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

public class SkeletonArchIdel : BaseIdel
{
    public override void OnStateStart()
    {
        enemy.rb2D.linearVelocity = Vector2.zero;
        enemy.ani.Play("SkeletonArchIdel");
    }
    public override void OnStateFixedUpdate()
    {

    }
    public override void OnStateUpdate()
    {
        SkeletonArch sa = GetEnemyAs<SkeletonArch>();
        sa.Flip();
        if (sa.recognitionModule.Recognize(enemy.monsterData.recognitionRange)&&Vector2.Distance(sa.transform.position,sa.target.transform.position)>10f)
        {
            sa.fsm.ChangeState(sa.fsm.states["Walk"]);
        }
        else
        {
            sa.fsm.ChangeState(sa.fsm.states["Attack"]);
        }
    }
    public override void OnStateEnd()
    {

    }
}

public class SkeletonArchWaitCooldown : State
{
    float waitTime = 0f;
    float maxWaitTime = 4f;

    public override void OnStateStart()
    {
        enemy.ani.Play("SkeletonArchIdel");
        waitTime = 0f;
        enemy.rb2D.linearVelocity = Vector2.zero;
    }

    public override void OnStateFixedUpdate()
    {

    }

    public override void OnStateUpdate()
    {
        var sa = GetEnemyAs<SkeletonArch>();
        waitTime += Time.deltaTime;
        sa.Flip();

        if (!sa.recognitionModule.Recognize(enemy.monsterData.recognitionRange))
        {
            sa.fsm.ChangeState(sa.fsm.states["Idel"]);
            return;
        }

        if (sa.canAttack && sa.recognitionModule.Recognize(10f))
        {
            sa.fsm.ChangeState(sa.fsm.states["Attack"]);
            return;
        }

        if (waitTime > maxWaitTime)
        {
            sa.fsm.ChangeState(sa.fsm.states["Idel"]);
        }
    }

    public override void OnStateEnd()
    {

    }
}

public class SkeletonArchAttack : BaseAttack
{
    AnimatorStateInfo currAni;
    bool usedSkill = false;
    float deg = 0f;

    public override void OnStateStart()
    {
        var sa = GetEnemyAs<SkeletonArch>();
        if (sa.canAttack)
        {
            usedSkill = false;
            if (!sa.isCurrupted)
            {
                sa.Attacked();
            }
            sa.rb2D.linearVelocity = Vector2.zero;
            sa.attackDir.SetActive(true);
            enemy.ani.Play("SkeletonArchAttack");
        }
        else
        {
            sa.fsm.ChangeState(sa.fsm.states["WaitCooldown"]);
        }
    }

    public override void OnStateFixedUpdate()
    {

    }

    public override void OnStateUpdate()
    {
        var sa = GetEnemyAs<SkeletonArch>();
        currAni = sa.ani.GetCurrentAnimatorStateInfo(0);

        if (currAni.normalizedTime >= 1.0f)
        {
            sa.fsm.ChangeState(enemy.fsm.states["Idel"]);
        }
        else if (currAni.normalizedTime >= 0.5f)
        {
            if (!usedSkill)
            {
                usedSkill = true;
                var o = ObjectPooler.Instance.Get(sa.arrow, sa.transform.position,new Vector3(0,0,deg));
                o.transform.position = sa.gameObject.transform.position;
                o.transform.localScale = new Vector2(0.3f, 0.3f);
                o.transform.localRotation = Quaternion.Euler(0, 0, deg);
            }
        }
        else
        {
            sa.dir = sa.recognitionModule.SolveDirection(sa.target.transform.position, sa.transform.position);
            deg = Mathf.Atan2(sa.dir.y, sa.dir.x) * Mathf.Rad2Deg;


            if (sa.transform.localScale.x < 0)
            {
                Vector2 flippedDir = new Vector2(-sa.dir.x, sa.dir.y);
                float flippedDeg = Mathf.Atan2(flippedDir.y, flippedDir.x) * Mathf.Rad2Deg;
                sa.attackDir.transform.localRotation = Quaternion.Euler(0, 0, flippedDeg - 90);
            }
            else
            {
                sa.attackDir.transform.localRotation = Quaternion.Euler(0, 0, deg - 90);
            }
        }
    }

    public override void OnStateEnd()
    {
        var sa = GetEnemyAs<SkeletonArch>();
        sa.attackDir.SetActive(false);
        sa.rb2D.linearVelocity = Vector2.zero;
    }
}

public class SkeletonArchStun : BaseStun
{
    float i;
    public override void OnStateStart()
    {
        enemy.ani.Play("SkeletonArchStun");
        i = 0;
        enemy.isStunning = true;
        enemy.staminaPoint.sr.color = Color.black;
    }
    public override void OnStateFixedUpdate()
    {

    }
    public override void OnStateUpdate()
    {
        var sa = GetEnemyAs<SkeletonArch>();
        i += Time.deltaTime;
        sa.Flip();
        sa.rb2D.linearVelocity = Vector2.zero;
        if (i > 2f)
        {
            sa.fsm.ChangeState(enemy.fsm.states["Idel"]);
        }
    }
    public override void OnStateEnd()
    {
        enemy.isStunning = false;
        enemy.CurrentStamina = enemy.monsterData.maxStamina;
    }
}