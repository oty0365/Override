using System.Collections;
using UnityEngine;

public class GoblinKnight : Enemy
{
    public APoolingObject colideParticle;

    [Header("모듈")]
    public PathFinderModule finderModule;
    public RecognitionModule recognitionModule;

    public GameObject spear;
    public GameObject attackDir;
    public Vector2 dir;

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
        spear.SetActive(false);
        attackDir.SetActive(false);
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
        var waitCooldown = new GoblinKnightWaitCooldown();
        fsm.AddState("Walk", walk, this);
        fsm.AddState("Death", death, this);
        fsm.AddState("Idel", idel, this);
        fsm.AddState("Attack", attack, this);
        fsm.AddState("Stun", stun, this);
        fsm.AddState("Hit", hit, this);
        fsm.AddState("Aim", aim, this);
        fsm.AddState("WaitCooldown", waitCooldown, this); 
        fsm.ChangeState(fsm.states["Idel"]);
    }
    public void OnDeath()
    {

        Debug.Log("이것때문에 죽음");
        StartCoroutine(DeathFlow());
    }
    private IEnumerator DeathFlow()
    {
        yield return new WaitForSeconds(4f);
        Death();
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
        gb.Flip();
        if (!gb.finderModule.CheckMoveDist() || !gb.recognitionModule.Recognize(enemy.monsterData.recognitionRange))
        {
            gb.fsm.ChangeState(gb.fsm.states["Idel"]);
        }
        if (gb.recognitionModule.Recognize(2.8f))
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
        enemy.ani.Play("GoblinKnightDeath");
        GetEnemyAs<GoblinKnight>().OnDeath();
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

public class GoblinKnightWaitCooldown : State
{
    float waitTime = 0f;
    float maxWaitTime = 1.0f;

    public override void OnStateStart()
    {
        enemy.ani.Play("GoblinKnightIdel");
        waitTime = 0f;
        enemy.rb2D.linearVelocity = Vector2.zero;
    }

    public override void OnStateFixedUpdate()
    {

    }

    public override void OnStateUpdate()
    {
        var gb = GetEnemyAs<GoblinKnight>();
        waitTime += Time.deltaTime;
        gb.Flip();

        if (!gb.recognitionModule.Recognize(enemy.monsterData.recognitionRange))
        {
            gb.fsm.ChangeState(gb.fsm.states["Idel"]);
            return;
        }

        if (gb.canAttack && gb.recognitionModule.Recognize(2.8f))
        {
            gb.fsm.ChangeState(gb.fsm.states["Aim"]);
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

public class GoblinKnightAim : BaseAttack
{
    float i = 0f;
    int redayaim = 1;
    public override void OnStateStart()
    {
        var gb = GetEnemyAs<GoblinKnight>();
        if (gb.canAttack)
        {
            enemy.ani.Play("GoblinKnightAttack");

            gb.dir = gb.recognitionModule.SolveDirection(gb.target.transform.position, gb.transform.position);
            if (gb.transform.localScale.x < 0)
            {
                gb.dir = new Vector2(-gb.dir.x, gb.dir.y);
            }
            var deg = Mathf.Atan2(gb.dir.y, gb.dir.x) * Mathf.Rad2Deg;
            gb.attackDir.gameObject.transform.localRotation = Quaternion.Euler(0, 0, deg - 90);
            gb.attackDir.SetActive(true);
            i = 0f;
            redayaim = 1;
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
        var gb = GetEnemyAs<GoblinKnight>();
        i += Time.deltaTime;
        gb.rb2D.linearVelocity = Vector2.zero;
        if (i > 0.35f)
        {
            if (redayaim > 1)
            {
                gb.fsm.ChangeState(enemy.fsm.states["Attack"]);
            }
        }
        if (redayaim <= 1)
        {
            gb.dir = gb.recognitionModule.SolveDirection(gb.target.transform.position, gb.transform.position);
            if (gb.transform.localScale.x < 0)
            {
                gb.dir = new Vector2(-gb.dir.x, gb.dir.y);
            }
            var deg = Mathf.Atan2(gb.dir.y, gb.dir.x) * Mathf.Rad2Deg;
            gb.attackDir.gameObject.transform.localRotation = Quaternion.Euler(0, 0, deg - 90);
        }
        if (i > 1f)
        {
            gb.Flip();

            if (redayaim <= 1)
            {
                i = 0;
                redayaim++;
            }
        }
    }
    public override void OnStateEnd()
    {
        GetEnemyAs<GoblinKnight>().attackDir.SetActive(false);
    }
}
public class GoblinKnightAttack : BaseAttack
{
    float i = 0f;
    bool isUsingRaycast = false;
    Vector2 targetPosition;
    float moveSpeed = 13.5f;
    float wallCheckDistance = 2f;

    public override void OnStateStart()
    {
        var gb = GetEnemyAs<GoblinKnight>();
        if (!gb.isCurrupted)
        {
            gb.Attacked();
        }

        Vector2 finalDirection = gb.dir;
        if (gb.transform.localScale.x < 0)
        {
            finalDirection = new Vector2(-gb.dir.x, gb.dir.y);
        }

        gb.spear.SetActive(true);
        enemy.ani.Play("GoblinKnightFire");
        i = 0f;

        RaycastHit2D nearWallHit = Physics2D.Raycast(gb.transform.position, finalDirection, wallCheckDistance, LayerMask.GetMask("Wall"));

        Vector2 dashDirection;

        if (nearWallHit.collider != null)
        {
            dashDirection = -finalDirection;
            float backoffDistance = 1.5f;

            isUsingRaycast = true;
            targetPosition = (Vector2)gb.transform.position + dashDirection * backoffDistance;
            gb.rb2D.linearVelocity = Vector2.zero;
        }
        else
        {
            dashDirection = finalDirection;
            float attackDistance = 100f;
            RaycastHit2D hit = Physics2D.Raycast(gb.transform.position, dashDirection, attackDistance, LayerMask.GetMask("Wall"));

            if (hit.collider != null)
            {
                isUsingRaycast = true;
                targetPosition = hit.point - dashDirection * 1f;
                gb.rb2D.linearVelocity = Vector2.zero;
            }
            else
            {
                isUsingRaycast = false;
                gb.rb2D.linearVelocity = dashDirection * moveSpeed;
            }
        }
        var dashDeg = Mathf.Atan2(dashDirection.y, dashDirection.x) * Mathf.Rad2Deg;

        Vector2 spearPosition;
        if (gb.transform.localScale.x < 0)
        {
            spearPosition = new Vector2(-dashDirection.x, dashDirection.y).normalized * 1.2f;
            dashDeg = 180f - dashDeg;
        }
        else
        {
            spearPosition = dashDirection.normalized * 1.2f;
        }

        gb.spear.transform.localPosition = spearPosition;
        gb.spear.transform.localRotation = Quaternion.Euler(0, 0, dashDeg);
    }

    public override void OnStateFixedUpdate()
    {
        if (isUsingRaycast)
        {
            var gb = GetEnemyAs<GoblinKnight>();
            Vector2 currentPos = gb.transform.position;
            Vector2 newPos = Vector2.MoveTowards(currentPos, targetPosition, moveSpeed * Time.fixedDeltaTime);
            gb.rb2D.MovePosition(newPos);

            if (Vector2.Distance(currentPos, targetPosition) < 0.5f)
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
        if (i > 0.6f)
        {
            gb.fsm.ChangeState(enemy.fsm.states["Idel"]);
        }
    }

    public override void OnStateEnd()
    {
        var gb = GetEnemyAs<GoblinKnight>();
        gb.rb2D.linearVelocity = Vector2.zero;
        gb.spear.SetActive(false);
    }
}
public class GoblinKnightStun : BaseStun
{
    float i;
    public override void OnStateStart()
    {
        enemy.ani.Play("GoblinKnightStun");
        i = 0;
        enemy.isStunning = true;
        enemy.staminaPoint.sr.color = Color.black;
    }
    public override void OnStateFixedUpdate()
    {

    }
    public override void OnStateUpdate()
    {
        var gb = GetEnemyAs<GoblinKnight>();
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