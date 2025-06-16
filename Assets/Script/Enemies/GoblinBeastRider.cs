using System.Collections;
using UnityEngine;

public class GoblinBeastRider : Enemy
{
    public APoolingObject collideParticle;

    [Header("¸ðµâ")]
    public PathFinderModule finderModule;
    public RecognitionModule recognitionModule;

    public GameObject spear;
    public GameObject attackDir;
    public APoolingObject grbite;
    public Transform center;
    public APoolingObject greenGoblin;
    public ParticleSystem prt;
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

    public override void Hit(Collider2D collider, float damage, float inflateTime)
    {
        /*if (fsm.currentState == fsm.states["Walk"] && fsm.currentState != fsm.states["Death"])
        {
            fsm.ChangeState(fsm.states["Hit"]);
        }*/
        base.Hit(collider, damage, inflateTime);
        PlayerCamera.Instance.SetShake(0.2f, 7.5f, 0.13f);
        ObjectPooler.Instance.Get(collideParticle, gameObject.transform.position, new Vector3(0, 0, 0), new Vector2(1f, 1f));
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
        var walk = new GoblinBeastRiderWalk();
        var death = new GoblinBeastRiderDeath();
        var idel = new GoblinBeastRiderIdel();
        var attack = new GoblinBeastRiderAttack();
        var stun = new GoblinBeastRiderStun();
        var hit = new GoblinBeastRiderHit();
        var aim = new GoblinBeastRiderAim();
        var waitCooldown = new GoblinBeastRiderWaitCooldown();
        var roar = new GoblinBeastRiderRoar();
        var gotoCenter = new GoblinBeastRiderGotoCenter();
        var bite = new GoblinBeastRiderBite();

        fsm.AddState("Walk", walk, this);
        fsm.AddState("Death", death, this);
        fsm.AddState("Idel", idel, this);
        fsm.AddState("Attack", attack, this);
        fsm.AddState("Stun", stun, this);
        fsm.AddState("Hit", hit, this);
        fsm.AddState("Aim", aim, this);
        fsm.AddState("Roar", roar, this);
        fsm.AddState("GotoCenter", gotoCenter, this);
        fsm.AddState("Bite", bite, this);
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

    public void CheckPattern()
    {
        if (target == null)
        {
            return;
        }

        if (canAttack)
        {
            if (MapManager.Instance.CurrentMonsters <= 1)
            {
                Debug.Log(battleManager.MonsterCount);
                fsm.ChangeState(fsm.states["GotoCenter"]);
                return;
            }

            float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);

            if (distanceToTarget > 6.5f)
            {
                fsm.ChangeState(fsm.states["Aim"]);
            }
            else if (distanceToTarget > 2f)
            {
                var randomAttack = Random.Range(0, 6);
                if (randomAttack > 2)
                {
                    fsm.ChangeState(fsm.states["Aim"]);
                }
                else
                {
                    fsm.ChangeState(fsm.states["Bite"]);
                }
            }

            else
            {
                var randomAttack = Random.Range(0, 10);
                if (randomAttack > 7)
                {
                    fsm.ChangeState(fsm.states["Aim"]);
                }
                else
                {
                    fsm.ChangeState(fsm.states["Bite"]);
                }
            }
        }
        else
        {
            fsm.ChangeState(fsm.states["WaitCooldown"]);
        }
    }
}

public class GoblinBeastRiderHit : State
{
    float timer;

    public override void OnStateStart()
    {
        enemy.ani.Play("GoblinBeastRiderIdel");
        timer = 0;
    }

    public override void OnStateFixedUpdate()
    {
    }

    public override void OnStateUpdate()
    {
        var gb = GetEnemyAs<GoblinBeastRider>();
        timer += Time.deltaTime;
        gb.Flip();
        gb.rb2D.linearVelocity = Vector2.zero;

        if (timer > 0.2f)
        {
            gb.fsm.ChangeState(enemy.fsm.states["Idel"]);
        }
    }

    public override void OnStateEnd()
    {
    }
}

public class GoblinBeastRiderWalk : State
{
    public override void OnStateStart()
    {
        enemy.ani.Play("GoblinBeastRiderWalk");
        GetEnemyAs<GoblinBeastRider>().finderModule.SetMove();
    }

    public override void OnStateFixedUpdate()
    {
    }

    public override void OnStateUpdate()
    {
        GoblinBeastRider gb = GetEnemyAs<GoblinBeastRider>();
        gb.Flip();

        if (!gb.finderModule.CheckMoveDist() || !gb.recognitionModule.Recognize(enemy.monsterData.recognitionRange))
        {
            gb.fsm.ChangeState(gb.fsm.states["Idel"]);
            return;
        }

        gb.CheckPattern();
    }

    public override void OnStateEnd()
    {
        GetEnemyAs<GoblinBeastRider>().finderModule.Stop();
    }
}

public class GoblinBeastRiderDeath : BaseDeath
{
    public override void OnStateStart()
    {
        enemy.ani.Play("GoblinBeastRiderDeath");
        GetEnemyAs<GoblinBeastRider>().OnDeath();
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

public class GoblinBeastRiderIdel : BaseIdel
{
    public override void OnStateStart()
    {
        enemy.rb2D.linearVelocity = Vector2.zero;
        enemy.ani.Play("GoblinBeastRiderIdel");
    }

    public override void OnStateFixedUpdate()
    {
    }

    public override void OnStateUpdate()
    {
        GoblinBeastRider gb = GetEnemyAs<GoblinBeastRider>();
        gb.Flip();

        if (gb.recognitionModule.Recognize(enemy.monsterData.recognitionRange))
        {
            gb.fsm.ChangeState(gb.fsm.states["Walk"]);
            return;
        }

        gb.CheckPattern();
    }

    public override void OnStateEnd()
    {
    }
}

public class GoblinBeastRiderWaitCooldown : State
{
    float waitTime = 0f;
    float maxWaitTime = 1.0f;

    public override void OnStateStart()
    {
        enemy.ani.Play("GoblinBeastRiderIdel");
        waitTime = 0f;
        enemy.rb2D.linearVelocity = Vector2.zero;
    }

    public override void OnStateFixedUpdate()
    {
    }

    public override void OnStateUpdate()
    {
        var gb = GetEnemyAs<GoblinBeastRider>();
        waitTime += Time.deltaTime;
        gb.Flip();

        if (!gb.recognitionModule.Recognize(enemy.monsterData.recognitionRange))
        {
            gb.fsm.ChangeState(gb.fsm.states["Idel"]);
            return;
        }
        if (gb.canAttack)
        {
            gb.CheckPattern();
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

public class GoblinBeastRiderAim : BaseAttack
{
    float timer = 0f;
    int readyAim = 1;
    Vector2 originalTargetDirection;

    public override void OnStateStart()
    {
        var gb = GetEnemyAs<GoblinBeastRider>();
        enemy.ani.Play("GoblinBeastRiderAim");

        originalTargetDirection = (gb.target.transform.position - gb.transform.position).normalized;
        gb.dir = gb.recognitionModule.SolveDirection(gb.target.transform.position, gb.transform.position);

        if (gb.transform.localScale.x < 0)
        {
            gb.dir = new Vector2(-gb.dir.x, gb.dir.y);
        }

        var deg = Mathf.Atan2(gb.dir.y, gb.dir.x) * Mathf.Rad2Deg;
        gb.attackDir.gameObject.transform.localRotation = Quaternion.Euler(0, 0, deg - 90);
        gb.attackDir.SetActive(true);
        timer = 0f;
        readyAim = 1;
    }

    public override void OnStateFixedUpdate()
    {
    }

    public override void OnStateUpdate()
    {
        var gb = GetEnemyAs<GoblinBeastRider>();
        timer += Time.deltaTime;
        gb.rb2D.linearVelocity = Vector2.zero;

        if (timer > 0.35f && readyAim > 1)
        {
            gb.fsm.ChangeState(enemy.fsm.states["Attack"]);
            return;
        }

        if (readyAim <= 1)
        {
            gb.dir = gb.recognitionModule.SolveDirection(gb.target.transform.position, gb.transform.position);
            if (gb.transform.localScale.x < 0)
            {
                gb.dir = new Vector2(-gb.dir.x, gb.dir.y);
            }

            var deg = Mathf.Atan2(gb.dir.y, gb.dir.x) * Mathf.Rad2Deg;
            gb.attackDir.gameObject.transform.localRotation = Quaternion.Euler(0, 0, deg - 90);
        }

        if (timer > 1f && readyAim <= 1)
        {
            float previousScaleX = gb.transform.localScale.x;
            gb.Flip();

            if (gb.transform.localScale.x != previousScaleX)
            {
                gb.dir = originalTargetDirection;
                if (gb.transform.localScale.x < 0)
                {
                    gb.dir = new Vector2(-gb.dir.x, gb.dir.y);
                }

                var deg = Mathf.Atan2(gb.dir.y, gb.dir.x) * Mathf.Rad2Deg;
                gb.attackDir.gameObject.transform.localRotation = Quaternion.Euler(0, 0, deg - 90);
            }

            timer = 0;
            readyAim++;
        }
    }

    public override void OnStateEnd()
    {
        GetEnemyAs<GoblinBeastRider>().attackDir.SetActive(false);
    }
}

public class GoblinBeastRiderAttack : BaseAttack
{
    float timer = 0f;
    bool isUsingRaycast = false;
    Vector2 targetPosition;
    float moveSpeed = 30f;
    float wallCheckDistance = 2f;
    Vector2 dashDirection;

    public override void OnStateStart()
    {
        var gb = GetEnemyAs<GoblinBeastRider>();
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
        enemy.ani.Play("GoblinBeastRiderDash");
        timer = 0f;

        RaycastHit2D nearWallHit = Physics2D.Raycast(gb.transform.position, finalDirection, wallCheckDistance, LayerMask.GetMask("Wall"));

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
            spearPosition = new Vector2(-dashDirection.x * 0.6f, dashDirection.y * 0.6f);
            dashDeg = 180f - dashDeg;
        }
        else
        {
            spearPosition = new Vector2(dashDirection.x * 0.6f, dashDirection.y * 0.6f);
        }

        gb.spear.transform.localPosition = spearPosition;
        gb.spear.transform.localRotation = Quaternion.Euler(0, 0, dashDeg);
    }

    public override void OnStateFixedUpdate()
    {
        var gb = GetEnemyAs<GoblinBeastRider>();

        if (isUsingRaycast)
        {
            Vector2 currentPos = gb.transform.position;
            Vector2 newPos = Vector2.MoveTowards(currentPos, targetPosition, moveSpeed * Time.fixedDeltaTime);
            gb.rb2D.MovePosition(newPos);

            if (Vector2.Distance(currentPos, targetPosition) < 0.5f)
            {
                gb.rb2D.linearVelocity = Vector2.zero;
                gb.fsm.ChangeState(enemy.fsm.states["Idel"]);
            }
        }
        else
        {
            RaycastHit2D wallHit = Physics2D.Raycast(gb.transform.position, dashDirection, 1f, LayerMask.GetMask("Wall"));
            if (wallHit.collider != null)
            {
                gb.rb2D.linearVelocity = Vector2.zero;
                gb.fsm.ChangeState(enemy.fsm.states["Idel"]);
            }
        }
    }

    public override void OnStateUpdate()
    {
        var gb = GetEnemyAs<GoblinBeastRider>();
        timer += Time.deltaTime;

        if (timer > 0.6f)
        {
            gb.fsm.ChangeState(enemy.fsm.states["Idel"]);
        }
    }

    public override void OnStateEnd()
    {
        var gb = GetEnemyAs<GoblinBeastRider>();
        gb.rb2D.linearVelocity = Vector2.zero;
        gb.spear.SetActive(false);
    }
}

public class GoblinBeastRiderStun : BaseStun
{
    float timer;

    public override void OnStateStart()
    {
        enemy.ani.Play("GoblinBeastRiderStun");
        timer = 0;
        enemy.isStunning = true;
        enemy.staminaPoint.sr.color = Color.black;
    }

    public override void OnStateFixedUpdate()
    {
    }

    public override void OnStateUpdate()
    {
        var gb = GetEnemyAs<GoblinBeastRider>();
        timer += Time.deltaTime;
        gb.Flip();
        gb.rb2D.linearVelocity = Vector2.zero;

        if (timer > 3f)
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

public class GoblinBeastRiderGotoCenter : BaseStun
{
    public override void OnStateStart()
    {
        var gb = GetEnemyAs<GoblinBeastRider>();
        enemy.ani.Play("GoblinBeastRiderWalk");
        gb.target = gb.center.gameObject;
    }

    public override void OnStateFixedUpdate()
    {
    }

    public override void OnStateUpdate()
    {
        var gb = GetEnemyAs<GoblinBeastRider>();

        if (Vector2.Distance(gb.transform.position, gb.center.position) > 0.1f)
        {
            gb.rb2D.MovePosition(Vector2.MoveTowards(gb.transform.position, gb.center.position, gb.monsterData.moveSpeed * Time.deltaTime*3f));
        }
        else
        {
            gb.target = PlayerInfo.Instance.gameObject;
            gb.fsm.ChangeState(enemy.fsm.states["Roar"]);
        }
    }

    public override void OnStateEnd()
    {
    }
}

public class GoblinBeastRiderRoar : BaseStun
{
    float timer;
    bool roared;

    public override void OnStateStart()
    {
        enemy.ani.Play("GoblinBeastRiderRoar");
        timer = 0;
        roared = false;
    }

    public override void OnStateFixedUpdate()
    {
    }

    public override void OnStateUpdate()
    {
        var gb = GetEnemyAs<GoblinBeastRider>();
        gb.rb2D.linearVelocity = Vector2.zero;
        timer += Time.deltaTime;

        if (timer > 0.26f && !roared)
        {
            roared = true;
            int[] x = new int[] { -1, 1, 1, -1 };
            int[] y = new int[] { -1, 1, -1, 1 };

            for (var i = 0; i < 4; i++)
            {
                var generatePos = (Vector2)gb.transform.position + new Vector2(3 * x[i], 3 * y[i]);
                var a = ObjectPooler.Instance.Get(gb.greenGoblin, generatePos, new Vector3(0, 0, 0));
                a.GetComponent<Enemy>().battleManager = gb.battleManager;
                gb.battleManager.MonsterCount++;
            }
            gb.prt.Clear();
            gb.prt.Play();

            PlayerCamera.Instance.SetShake(0.6f, 8f, 0.5f);
        }

        if (timer > 1.1f)
        {
            gb.fsm.ChangeState(enemy.fsm.states["Idel"]);
        }
    }

    public override void OnStateEnd()
    {
        var gb = GetEnemyAs<GoblinBeastRider>();
        gb.prt.Stop();
    }
}

public class GoblinBeastRiderBite : BaseAttack
{
    AnimatorStateInfo currAni;

    public override void OnStateStart()
    {
        var gb = GetEnemyAs<GoblinBeastRider>();
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

        var o = ObjectPooler.Instance.Get(gb.grbite, gb.transform);
        o.transform.localScale = new Vector2(1f, 1f);
        o.transform.localPosition = gb.dir * 0.9f;
        o.transform.localRotation = Quaternion.Euler(0, 0, deg);
        enemy.ani.Play("GoblinBeastRiderBite");
        gb.rb2D.linearVelocity = Vector2.zero;
    }

    public override void OnStateFixedUpdate()
    {
    }

    public override void OnStateUpdate()
    {
        currAni = enemy.ani.GetCurrentAnimatorStateInfo(0);
        var gb = GetEnemyAs<GoblinBeastRider>();

        if (currAni.normalizedTime >= 1f)
        {
            gb.fsm.ChangeState(gb.fsm.states["Idel"]);
        }
    }

    public override void OnStateEnd()
    {
        var gb = GetEnemyAs<GoblinBeastRider>();
        gb.attackDir.SetActive(false);
        gb.rb2D.linearVelocity = Vector2.zero;
    }
}