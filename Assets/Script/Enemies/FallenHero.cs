using System.Collections;
using UnityEngine;

public class FallenHero : Enemy
{
    public APoolingObject collideParticle;

    [Header("모듈")]
    public PathFinderModule finderModule;
    public RecognitionModule recognitionModule;

    [Header("무기 및 공격")]
    public GameObject sword;
    public GameObject attackDir;
    public APoolingObject slashEffect;
    public APoolingObject dashEffect;
    public Transform center;
    public ParticleSystem auraParticle;
    public Vector2 dir;

    [Header("콤보 시스템")]
    public int currentComboCount = 0;
    public int maxComboCount = 3;
    public float comboResetTime = 2f;
    private float comboTimer = 0f;
    public bool isInCombo = false;

    protected override void Update()
    {
        base.Update();
        if (isStun && fsm.currentState != fsm.states["Death"])
        {
            fsm.ChangeState(fsm.states["Stun"]);
            isStun = false;
        }
        if (isInCombo)
        {
            comboTimer += Time.deltaTime;
            if (comboTimer >= comboResetTime)
            {
                ResetCombo();
            }
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Hit(Collider2D collider, float damage, float inflateTime)
    {
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
        yield return new WaitForSeconds(1.2f);
        canAttack = true;
    }

    public void Attacked()
    {
        StartCoroutine(AttackCoolDown());
    }

    public void ResetCombo()
    {
        currentComboCount = 0;
        isInCombo = false;
        comboTimer = 0f;
    }

    public void AdvanceCombo()
    {
        currentComboCount++;
        isInCombo = true;
        comboTimer = 0f;
    }

    public override void InitEnemy()
    {
        target = PlayerInfo.Instance.gameObject;
        sword.SetActive(false);
        attackDir.SetActive(false);
        base.InitEnemy();
        recognitionModule.Initialize();
        finderModule.Initialize();

        var walk = new FallenHeroWalk();
        var death = new FallenHeroDeath();
        var idle = new FallenHeroIdle();
        var slash1 = new FallenHeroSlash1();
        var slash2 = new FallenHeroSlash2();
        var slash3 = new FallenHeroSlash3();
        var dashAttack = new FallenHeroDashAttack();
        var approach = new FallenHeroApproach();
        var stun = new FallenHeroStun();
        var hit = new FallenHeroHit();
        var waitCooldown = new FallenHeroWaitCooldown();

        fsm.AddState("Walk", walk, this);
        fsm.AddState("Death", death, this);
        fsm.AddState("Idle", idle, this);
        fsm.AddState("Slash1", slash1, this);
        fsm.AddState("Slash2", slash2, this);
        fsm.AddState("Slash3", slash3, this);
        fsm.AddState("DashAttack", dashAttack, this);
        fsm.AddState("Approach", approach, this);
        fsm.AddState("Stun", stun, this);
        fsm.AddState("Hit", hit, this);
        fsm.AddState("WaitCooldown", waitCooldown, this);
        fsm.ChangeState(fsm.states["Idle"]);
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
        if (target == null || !canAttack)
        {
            fsm.ChangeState(fsm.states["WaitCooldown"]);
            return;
        }

        float distanceToTarget = Vector2.Distance(transform.position, target.transform.position);

        if (distanceToTarget > 6f)
        {
            fsm.ChangeState(fsm.states["Approach"]);
        }
        else if (distanceToTarget > 3f)
        {
            int randomChoice = Random.Range(0, 10);
            if (randomChoice > 3) // 대쉬 공격 확률 60%
            {
                fsm.ChangeState(fsm.states["DashAttack"]);
            }
            else
            {
                fsm.ChangeState(fsm.states["Approach"]);
            }
        }
        else
        {
            // 근거리에서는 대쉬 확률을 낮추고 일반 공격 위주로
            int dashChance = Random.Range(0, 10);
            if (dashChance < 2) // 20% 확률로 대쉬
            {
                fsm.ChangeState(fsm.states["DashAttack"]);
            }
            else
            {
                DecideComboAction(distanceToTarget);
            }
        }
    }

    private void DecideComboAction(float distance)
    {
        int randomPattern = Random.Range(0, 10);

        if (!isInCombo)
        {
            // 거리별 공격 가능 여부 체크
            bool canSlash1 = distance <= 2.5f;
            bool canSlash2 = distance <= 3f;
            bool canSlash3 = distance <= 4f;

            if (canSlash3 && randomPattern > 7) // slash3 (가장 넓은 범위)
            {
                fsm.ChangeState(fsm.states["Slash3"]);
            }
            else if (canSlash2 && randomPattern > 4) // slash2 (중간 범위)
            {
                fsm.ChangeState(fsm.states["Slash2"]);
            }
            else if (canSlash1) // slash1 (근거리)
            {
                fsm.ChangeState(fsm.states["Slash1"]);
            }
            else
            {
                // 공격이 불가능한 경우 접근
                fsm.ChangeState(fsm.states["Approach"]);
            }
        }
        else
        {
            if (currentComboCount == 1)
            {
                int choice = Random.Range(0, 10);
                bool canSlash1 = distance <= 2.5f;
                bool canSlash2 = distance <= 3f;
                bool canSlash3 = distance <= 4f;

                if (canSlash3 && choice > 6) // slash3
                {
                    fsm.ChangeState(fsm.states["Slash3"]);
                }
                else if (canSlash2 && choice > 4) // slash2
                {
                    fsm.ChangeState(fsm.states["Slash2"]);
                }
                else if (canSlash1) // slash1
                {
                    fsm.ChangeState(fsm.states["Slash1"]);
                }
                else
                {
                    fsm.ChangeState(fsm.states["Approach"]);
                }
            }
            else if (currentComboCount == 2)
            {
                int choice = Random.Range(0, 10);
                bool canSlash1 = distance <= 2.5f;
                bool canSlash2 = distance <= 3f;
                bool canSlash3 = distance <= 4f;

                if (canSlash3 && choice > 6) // slash3 마무리
                {
                    fsm.ChangeState(fsm.states["Slash3"]);
                }
                else if (canSlash2 && choice > 4) // slash2
                {
                    fsm.ChangeState(fsm.states["Slash2"]);
                }
                else if (canSlash1) // slash1
                {
                    fsm.ChangeState(fsm.states["Slash1"]);
                }
                else
                {
                    ResetCombo();
                    fsm.ChangeState(fsm.states["Approach"]);
                }
            }
            else
            {
                ResetCombo();
                fsm.ChangeState(fsm.states["WaitCooldown"]);
            }
        }
    }
}

public class FallenHeroHit : State
{
    float timer;

    public override void OnStateStart()
    {
        enemy.ani.Play("FallenHeroIdle");
        timer = 0;
    }

    public override void OnStateFixedUpdate()
    {
    }

    public override void OnStateUpdate()
    {
        var fh = GetEnemyAs<FallenHero>();
        timer += Time.deltaTime;
        fh.Flip();
        fh.rb2D.linearVelocity = Vector2.zero;

        if (timer > 0.2f)
        {
            fh.fsm.ChangeState(enemy.fsm.states["Idle"]);
        }
    }

    public override void OnStateEnd()
    {
    }
}

public class FallenHeroWalk : State
{
    public override void OnStateStart()
    {
        enemy.ani.Play("FallenHeroWalk");
        GetEnemyAs<FallenHero>().finderModule.SetMove();
    }

    public override void OnStateFixedUpdate()
    {
    }

    public override void OnStateUpdate()
    {
        FallenHero fh = GetEnemyAs<FallenHero>();
        fh.Flip();

        if (!fh.finderModule.CheckMoveDist() || !fh.recognitionModule.Recognize(enemy.monsterData.recognitionRange))
        {
            fh.fsm.ChangeState(fh.fsm.states["Idle"]);
            return;
        }

        fh.CheckPattern();
    }

    public override void OnStateEnd()
    {
        GetEnemyAs<FallenHero>().finderModule.Stop();
    }
}

public class FallenHeroDeath : BaseDeath
{
    public override void OnStateStart()
    {
        enemy.ani.Play("FallenHeroDeath");
        GetEnemyAs<FallenHero>().OnDeath();
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

public class FallenHeroIdle : BaseIdel
{
    public override void OnStateStart()
    {
        enemy.rb2D.linearVelocity = Vector2.zero;
        enemy.ani.Play("FallenHeroIdle");
    }

    public override void OnStateFixedUpdate()
    {
    }

    public override void OnStateUpdate()
    {
        FallenHero fh = GetEnemyAs<FallenHero>();
        fh.Flip();

        if (fh.recognitionModule.Recognize(enemy.monsterData.recognitionRange))
        {
            fh.CheckPattern();
            return;
        }
    }

    public override void OnStateEnd()
    {
    }
}

public class FallenHeroWaitCooldown : State
{
    float waitTime = 0f;
    float maxWaitTime = 1.0f;

    public override void OnStateStart()
    {
        enemy.ani.Play("FallenHeroIdle");
        waitTime = 0f;
        enemy.rb2D.linearVelocity = Vector2.zero;
    }

    public override void OnStateFixedUpdate()
    {
    }

    public override void OnStateUpdate()
    {
        var fh = GetEnemyAs<FallenHero>();
        waitTime += Time.deltaTime;
        fh.Flip();

        if (!fh.recognitionModule.Recognize(enemy.monsterData.recognitionRange))
        {
            fh.fsm.ChangeState(fh.fsm.states["Idle"]);
            return;
        }
        if (fh.canAttack)
        {
            fh.CheckPattern();
            return;
        }

        if (waitTime > maxWaitTime)
        {
            fh.fsm.ChangeState(fh.fsm.states["Walk"]);
        }
    }

    public override void OnStateEnd()
    {
    }
}

public class FallenHeroApproach : State
{
    float approachTimer = 0f;
    float maxApproachTime = 2f;

    public override void OnStateStart()
    {
        enemy.ani.Play("FallenHeroWalk");
        approachTimer = 0f;
    }

    public override void OnStateFixedUpdate()
    {
        var fh = GetEnemyAs<FallenHero>();
        Vector2 direction = (fh.target.transform.position - fh.transform.position).normalized;
        fh.rb2D.MovePosition((Vector2)fh.transform.position + direction * fh.monsterData.moveSpeed * 1.5f * Time.fixedDeltaTime);
    }

    public override void OnStateUpdate()
    {
        var fh = GetEnemyAs<FallenHero>();
        approachTimer += Time.deltaTime;
        fh.Flip();

        float distance = Vector2.Distance(fh.transform.position, fh.target.transform.position);

        if (distance <= 3f)
        {
            fh.CheckPattern();
            return;
        }

        if (approachTimer > maxApproachTime)
        {
            fh.fsm.ChangeState(fh.fsm.states["Idle"]);
        }
    }

    public override void OnStateEnd()
    {
        enemy.rb2D.linearVelocity = Vector2.zero;
    }
}

public class FallenHeroSlash1 : BaseAttack
{
    AnimatorStateInfo currAni;

    public override void OnStateStart()
    {
        var fh = GetEnemyAs<FallenHero>();
        if (!fh.isCurrupted)
        {
            fh.Attacked();
        }

        fh.AdvanceCombo();
        fh.dir = fh.recognitionModule.SolveDirection(fh.target.transform.position, fh.transform.position);

        var deg = Mathf.Atan2(fh.dir.y, fh.dir.x) * Mathf.Rad2Deg;
        fh.attackDir.SetActive(true);
        fh.attackDir.transform.localRotation = Quaternion.Euler(0, 0, deg - 90);

        fh.sword.SetActive(true);
        fh.sword.transform.localPosition = Vector3.zero;
        fh.sword.transform.localRotation = Quaternion.Euler(0, 0, (deg - 90) * fh.transform.localScale.x);

        var slashEffect = ObjectPooler.Instance.Get(fh.slashEffect, fh.transform);
        slashEffect.transform.localPosition = fh.dir * 1.2f;
        slashEffect.transform.localRotation = Quaternion.Euler(0, 0, deg);

        enemy.ani.Play("FallenHeroAttack1");
        fh.rb2D.linearVelocity = Vector2.zero;
    }

    public override void OnStateFixedUpdate()
    {
    }

    public override void OnStateUpdate()
    {
        currAni = enemy.ani.GetCurrentAnimatorStateInfo(0);
        var fh = GetEnemyAs<FallenHero>();

        if (currAni.normalizedTime >= 0.8f)
        {
            fh.CheckPattern();
        }
    }

    public override void OnStateEnd()
    {
        var fh = GetEnemyAs<FallenHero>();
        fh.attackDir.SetActive(false);
        fh.sword.SetActive(false);
    }
}

public class FallenHeroSlash2 : BaseAttack
{
    AnimatorStateInfo currAni;

    public override void OnStateStart()
    {
        var fh = GetEnemyAs<FallenHero>();
        fh.AdvanceCombo();
        fh.dir = fh.recognitionModule.SolveDirection(fh.target.transform.position, fh.transform.position);

        var deg = Mathf.Atan2(fh.dir.y, fh.dir.x) * Mathf.Rad2Deg;
        fh.attackDir.SetActive(true);
        fh.attackDir.transform.localRotation = Quaternion.Euler(0, 0, deg - 90);

        fh.sword.SetActive(true);
        fh.sword.transform.localPosition = Vector3.zero;
        fh.sword.transform.localRotation = Quaternion.Euler(0, 0, (deg + 45 - 90) * fh.transform.localScale.x);

        var slashEffect = ObjectPooler.Instance.Get(fh.slashEffect, fh.transform);
        slashEffect.transform.localPosition = fh.dir * 1.2f;
        slashEffect.transform.localRotation = Quaternion.Euler(0, 0, deg + 45);

        enemy.ani.Play("FallenHeroAttack2");
        fh.rb2D.linearVelocity = Vector2.zero;
    }

    public override void OnStateFixedUpdate()
    {
    }

    public override void OnStateUpdate()
    {
        currAni = enemy.ani.GetCurrentAnimatorStateInfo(0);
        var fh = GetEnemyAs<FallenHero>();

        if (currAni.normalizedTime >= 0.8f)
        {
            fh.CheckPattern();
        }
    }

    public override void OnStateEnd()
    {
        var fh = GetEnemyAs<FallenHero>();
        fh.attackDir.SetActive(false);
        fh.sword.SetActive(false);
    }
}

public class FallenHeroSlash3 : BaseAttack
{
    AnimatorStateInfo currAni;

    public override void OnStateStart()
    {
        var fh = GetEnemyAs<FallenHero>();
        fh.AdvanceCombo();
        fh.dir = fh.recognitionModule.SolveDirection(fh.target.transform.position, fh.transform.position);

        var deg = Mathf.Atan2(fh.dir.y, fh.dir.x) * Mathf.Rad2Deg;
        fh.attackDir.SetActive(true);
        fh.attackDir.transform.localRotation = Quaternion.Euler(0, 0, deg - 90);

        fh.sword.SetActive(true);
        fh.sword.transform.localPosition = Vector3.zero;
        fh.sword.transform.localRotation = Quaternion.Euler(0, 0, (deg - 90) * fh.transform.localScale.x);

        var slashEffect = ObjectPooler.Instance.Get(fh.slashEffect, fh.transform);
        slashEffect.transform.localPosition = fh.dir * 1.5f;
        slashEffect.transform.localRotation = Quaternion.Euler(0, 0, deg);
        slashEffect.transform.localScale = Vector3.one * 1.5f;

        enemy.ani.Play("FallenHeroAttack3");
        fh.rb2D.linearVelocity = Vector2.zero;

        PlayerCamera.Instance.SetShake(0.4f, 8f, 0.3f);
    }

    public override void OnStateFixedUpdate()
    {
    }

    public override void OnStateUpdate()
    {
        currAni = enemy.ani.GetCurrentAnimatorStateInfo(0);
        var fh = GetEnemyAs<FallenHero>();

        if (currAni.normalizedTime >= 1f)
        {
            fh.ResetCombo();
            fh.fsm.ChangeState(fh.fsm.states["WaitCooldown"]);
        }
    }

    public override void OnStateEnd()
    {
        var fh = GetEnemyAs<FallenHero>();
        fh.attackDir.SetActive(false);
        fh.sword.SetActive(false);
    }
}

public class FallenHeroDashAttack : BaseAttack
{
    float timer = 0f;
    bool isUsingRaycast = false;
    Vector2 targetPosition;
    float moveSpeed = 18f;
    float wallCheckDistance = 2f;
    Vector2 dashDirection;
    bool hasAttacked = false; // 공격 판정을 한 번만 하기 위한 플래그
    float attackStartTime = 0.2f; // 대쉬 시작 후 언제부터 공격 판정을 시작할지
    float attackEndTime = 0.4f; // 언제까지 공격 판정을 유지할지

    public override void OnStateStart()
    {
        var fh = GetEnemyAs<FallenHero>();
        if (!fh.isCurrupted)
        {
            fh.Attacked();
        }

        // 대쉬는 콤보에 포함시키지 않음
        hasAttacked = false;

        float distanceToTarget = Vector2.Distance(fh.transform.position, fh.target.transform.position);
        Vector2 finalDirection;

        if (distanceToTarget <= 1.5f && Random.Range(0, 10) < 3)
        {
            Vector2 playerDirection = (fh.target.transform.position - fh.transform.position).normalized;
            Vector2 perpendicularDir = new Vector2(-playerDirection.y, playerDirection.x);

            if (Random.Range(0, 2) == 0)
                perpendicularDir = -perpendicularDir;

            finalDirection = perpendicularDir;
        }
        else
        {
            finalDirection = fh.recognitionModule.SolveDirection(fh.target.transform.position, fh.transform.position);
        }

        fh.sword.SetActive(true);
        var dashEffect = ObjectPooler.Instance.Get(fh.dashEffect, fh.transform.position, Vector3.zero);

        enemy.ani.Play("FallenHeroDash");
        timer = 0f;

        RaycastHit2D nearWallHit = Physics2D.Raycast(fh.transform.position, finalDirection, wallCheckDistance, LayerMask.GetMask("Wall"));

        if (nearWallHit.collider != null)
        {
            dashDirection = -finalDirection;
            float backoffDistance = 1.5f;
            isUsingRaycast = true;
            targetPosition = (Vector2)fh.transform.position + dashDirection * backoffDistance;
            fh.rb2D.linearVelocity = Vector2.zero;
        }
        else
        {
            dashDirection = finalDirection;
            float attackDistance = 8f; // 대쉬 거리 조정
            RaycastHit2D hit = Physics2D.Raycast(fh.transform.position, dashDirection, attackDistance, LayerMask.GetMask("Wall"));

            if (hit.collider != null)
            {
                isUsingRaycast = true;
                targetPosition = hit.point - dashDirection * 1f;
                fh.rb2D.linearVelocity = Vector2.zero;
            }
            else
            {
                isUsingRaycast = false;
                fh.rb2D.linearVelocity = dashDirection * moveSpeed;
            }
        }

        var dashDeg = Mathf.Atan2(dashDirection.y, dashDirection.x) * Mathf.Rad2Deg;
        fh.sword.transform.localPosition = Vector3.zero;
        fh.sword.transform.localRotation = Quaternion.Euler(0, 0, (dashDeg + 90) * fh.transform.localScale.x);

        PlayerCamera.Instance.SetShake(0.3f, 10f, 0.2f);
    }

    public override void OnStateFixedUpdate()
    {
        var fh = GetEnemyAs<FallenHero>();

        if (isUsingRaycast)
        {
            Vector2 currentPos = fh.transform.position;
            Vector2 newPos = Vector2.MoveTowards(currentPos, targetPosition, moveSpeed * Time.fixedDeltaTime);
            fh.rb2D.MovePosition(newPos);

            if (Vector2.Distance(currentPos, targetPosition) < 0.5f)
            {
                fh.rb2D.linearVelocity = Vector2.zero;
                timer = 0.8f; // 강제로 타이머를 늘려서 끝나게 함
            }
        }
        else
        {
            RaycastHit2D wallHit = Physics2D.Raycast(fh.transform.position, dashDirection, 1f, LayerMask.GetMask("Wall"));
            if (wallHit.collider != null)
            {
                fh.rb2D.linearVelocity = Vector2.zero;
                timer = 0.8f; // 강제로 타이머를 늘려서 끝나게 함
            }
        }

        // 대쉬 중 플레이어와의 충돌 체크 (공격 판정)
        if (!hasAttacked && timer >= attackStartTime && timer <= attackEndTime)
        {
            float distanceToPlayer = Vector2.Distance(fh.transform.position, fh.target.transform.position);
            if (distanceToPlayer <= 1.5f) // 공격 범위
            {
                hasAttacked = true;
                // 여기서 플레이어에게 데미지를 주는 로직을 추가
                // 예: PlayerInfo.Instance.TakeDamage(fh.monsterData.attackDamage);
                PlayerCamera.Instance.SetShake(0.5f, 12f, 0.3f);
            }
        }
    }

    public override void OnStateUpdate()
    {
        var fh = GetEnemyAs<FallenHero>();
        timer += Time.deltaTime;

        if (timer > 0.3f) // 대쉬 지속 시간
        {
            fh.fsm.ChangeState(fh.fsm.states["WaitCooldown"]);
        }
    }

    public override void OnStateEnd()
    {
        var fh = GetEnemyAs<FallenHero>();
        fh.rb2D.linearVelocity = Vector2.zero;
        fh.sword.SetActive(false);
    }
}

public class FallenHeroStun : BaseStun
{
    float timer;

    public override void OnStateStart()
    {
        enemy.ani.Play("FallenHeroStun");
        timer = 0;
        enemy.isStunning = true;
        enemy.staminaPoint.sr.color = Color.black;
        GetEnemyAs<FallenHero>().ResetCombo();
    }

    public override void OnStateFixedUpdate()
    {
    }

    public override void OnStateUpdate()
    {
        var fh = GetEnemyAs<FallenHero>();
        timer += Time.deltaTime;
        fh.Flip();
        fh.rb2D.linearVelocity = Vector2.zero;

        if (timer > 3f)
        {
            fh.fsm.ChangeState(enemy.fsm.states["Idle"]);
        }
    }

    public override void OnStateEnd()
    {
        enemy.isStunning = false;
        enemy.CurrentStamina = enemy.monsterData.maxStamina;
    }
}