using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public abstract class Enemy : APoolingObject
{
    [Header("해당 배틀 매니져")]
    public BattleManager battleManager;
    [Header("몬스터 데이터")]
    public MonsterData monsterData;
    [Header("스테미나 포인트 위치")]
    public Transform staminaPointPos;
    [Header("몬스터 애니메이터")]
    public Animator ani;
    public SpriteRenderer sr;
    [Header("몬스터 물리연산")]
    public Collider2D enemyHitBox;
    public Rigidbody2D rb2D;
    protected MaterialPropertyBlock _metProps;
    protected Coroutine _currentHitFlow;
    public FSM fsm = new();
    public Dictionary<Collider2D, bool> contactWithDamage = new();

    // 다단히트 관리용 딕셔너리들
    private Dictionary<Collider2D, Coroutine> infinateCoroutines = new();
    private Dictionary<Collider2D, float> lastValidHitTime = new();

    protected Coroutine _currentInfinateFlow;
    [Header("공격할 대상")]
    public GameObject target;

    public bool isStun;
    public bool isStunning;
    public bool canAttack;
    public bool isCurrupted;

    public StaminaPoint staminaPoint;
    public GameObject glowEye;
    public GameObject backParticle;
    public Action onHitAction;
    public Action onDeathAction;

    [Header("히트 설정")]
    [SerializeField] private float minHitInterval = 0.1f; // 최소 히트 간격

    [SerializeField]
    private float _currentHp;
    public float CurrentHp
    {
        get => _currentHp;
        set
        {
            _currentHp = value;
            onHitAction?.Invoke();
            if (_currentHp <= 0)
            {
                DeathDrop();
            }
        }
    }

    [SerializeField]
    private float _currentStamia;
    public float CurrentStamina
    {
        get => _currentStamia;
        set
        {
            if (value > monsterData.maxStamina)
            {
                value = monsterData.maxStamina;
            }
            if (value < 0)
            {
                value = 0;
            }
            if (value != _currentStamia)
            {
                _currentStamia = value;
                if (staminaPoint != null)
                {
                    staminaPoint.OnvalueChange();
                }
            }
            Debug.Log(_currentStamia);
        }
    }

    protected void StateUpdate()
    {
        fsm.UpdateState();
    }
    protected void StateFixedUpdate()
    {
        fsm.FixedUpdateState();
    }

    public virtual void InitEnemy()
    {
        gameObject.SetActive(true);
        if (isCurrupted)
        {
            sr.color = Color.black;
            glowEye.SetActive(true);
            backParticle.SetActive(true);
        }
        else
        {
            sr.color = Color.white;
            if (glowEye != null)
            {
                glowEye.SetActive(false);
            }
            if (backParticle != null)
            {
                backParticle.SetActive(false);
            }

        }
        canAttack = true;
        isStun = false;
        isStunning = false;

        // 모든 관련 딕셔너리 초기화
        CleanupAllData();

        fsm.InitState();
        CurrentHp = monsterData.maxHp;
        CurrentStamina = monsterData.maxStamina;
        _metProps = new MaterialPropertyBlock();
        var a = ObjGenerator.Instance.generateDict["StaminaPoint"];
        var o = ObjectPooler.Instance.Get(a, staminaPointPos.transform.position, new Vector3(0, 0, 45));
        var c = o.GetComponent<StaminaPoint>();
        staminaPoint = c;
        c.currentEnemy = this;
    }

    // 모든 데이터를 정리하는 메서드
    private void CleanupAllData()
    {
        // 모든 실행 중인 코루틴 정리
        foreach (var coroutine in infinateCoroutines.Values)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }

        contactWithDamage.Clear();
        infinateCoroutines.Clear();
        lastValidHitTime.Clear();
    }

    // 오브젝트가 비활성화될 때 호출되는 메서드
    private void OnDisable()
    {
        CleanupAllData();
    }

    public void DeathDrop()
    {
        onHitAction = null;
        onDeathAction?.Invoke();
        onDeathAction = null;
        staminaPoint.Death();
        if (battleManager != null)
        {
            battleManager.MonsterCount--;
        }
        PlayerInfo.Instance.PlayerCoin += UnityEngine.Random.Range(monsterData.minCoin, monsterData.maxCoin + 1);
        if (isCurrupted)
        {
            backParticle.SetActive(false);
            glowEye.SetActive(false);
        }

        // 모든 데이터 정리
        CleanupAllData();
    }

    public void RemovedByGame()
    {
        battleManager.MonsterCount--;
        //staminaPoint.Death();
        Death();
    }

    public virtual void Hit(Collider2D collider, float damage, float infinateTime)
    {
        Debug.Log($"Hit called - Collider: {collider.name}, Damage: {damage}, Time: {Time.time}");

        if (staminaPoint != null)
        {
            if (!isStunning)
            {
                CurrentStamina -= 10f;
            }
            if (CurrentHp > 0)
            {
                CurrentHp -= damage;
            }
            if (_currentHitFlow != null)
            {
                StopCoroutine(_currentHitFlow);
            }
            _currentHitFlow = StartCoroutine(HitFlow());

            // 기존 코루틴이 있으면 중지하고 새로 시작
            if (infinateCoroutines.ContainsKey(collider) && infinateCoroutines[collider] != null)
            {
                StopCoroutine(infinateCoroutines[collider]);
            }
            infinateCoroutines[collider] = StartCoroutine(InfinateTimeFlow(collider, infinateTime));
        }
    }

    private IEnumerator HitFlow()
    {
        for (var i = 0f; i < 1f; i += Time.deltaTime * 8f)
        {
            sr.GetPropertyBlock(_metProps);
            _metProps.SetFloat("_Progress", i);
            sr.SetPropertyBlock(_metProps);
            yield return null;
        }
        sr.GetPropertyBlock(_metProps);
        _metProps.SetFloat("_Progress", 1);
        sr.SetPropertyBlock(_metProps);

        for (var i = 1f; i > 0f; i -= Time.deltaTime * 8f)
        {
            sr.GetPropertyBlock(_metProps);
            _metProps.SetFloat("_Progress", i);
            sr.SetPropertyBlock(_metProps);
            yield return null;
        }
        sr.GetPropertyBlock(_metProps);
        _metProps.SetFloat("_Progress", 0);
        sr.SetPropertyBlock(_metProps);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!contactWithDamage.ContainsKey(other) && other.CompareTag("damageable"))
        {
            contactWithDamage.Add(other, false);

            if (transform.childCount == 0)
            {
                other.GetComponent<AAttack>().CastDamage(this);
            }
            else
            {
                other.GetComponentInChildren<AAttack>().CastDamage(this);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("damageable"))
        {
            RemoveDamaging(other);
        }
    }

    public void RemoveDamaging(Collider2D other)
    {
        contactWithDamage?.Remove(other);

        // 관련 코루틴과 히트 시간도 정리
        if (infinateCoroutines.ContainsKey(other))
        {
            if (infinateCoroutines[other] != null)
            {
                StopCoroutine(infinateCoroutines[other]);
            }
            infinateCoroutines.Remove(other);
        }

        if (lastValidHitTime.ContainsKey(other))
        {
            lastValidHitTime.Remove(other);
        }
    }

    // null이거나 비활성화된 콜라이더들을 정리하는 메서드
    private void CleanupInvalidColliders()
    {
        var toRemove = new List<Collider2D>();

        foreach (var kvp in contactWithDamage)
        {
            if (kvp.Key == null || !kvp.Key.gameObject.activeInHierarchy)
            {
                toRemove.Add(kvp.Key);
            }
        }

        foreach (var collider in toRemove)
        {
            RemoveDamaging(collider);
        }
    }

    protected virtual void FixedUpdate()
    {
        StateFixedUpdate();

        // null 체크 및 비활성화된 오브젝트 정리
        CleanupInvalidColliders();

        foreach (var kvp in contactWithDamage.ToList())
        {
            if (kvp.Key != null && kvp.Key.gameObject.activeInHierarchy && !kvp.Value)
            {
                // 마지막 유효한 히트 이후 충분한 시간이 지났는지 확인
                if (!lastValidHitTime.ContainsKey(kvp.Key) ||
                    Time.time - lastValidHitTime[kvp.Key] >= minHitInterval)
                {
                    lastValidHitTime[kvp.Key] = Time.time;

                    if (transform.childCount == 0)
                    {
                        kvp.Key.GetComponent<AAttack>().CastDamage(this);
                    }
                    else
                    {
                        kvp.Key.GetComponentInChildren<AAttack>().CastDamage(this);
                    }
                }
            }
        }
    }

    protected virtual void Update()
    {
        StateUpdate();
    }

    protected IEnumerator InfinateTimeFlow(Collider2D collider, float infinateTime)
    {
        if (contactWithDamage.ContainsKey(collider))
        {
            contactWithDamage[collider] = true;
        }

        yield return new WaitForSeconds(infinateTime);

        if (contactWithDamage.ContainsKey(collider))
        {
            contactWithDamage[collider] = false;
        }

        // 코루틴 완료 후 딕셔너리에서 제거
        if (infinateCoroutines.ContainsKey(collider))
        {
            infinateCoroutines.Remove(collider);
        }
    }

    public void Flip()
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
    }
    /*protected void OnDestroy()
    {
        if (staminaPoint != null)
        {
            staminaPoint.Death();
        }
        Death();
    }*/

    /* void OnDisable()
     {
         Debug.Log($"{gameObject.name} disabled!");
         Debug.LogError(System.Environment.StackTrace);
     }

     void OnEnable()
     {
         Debug.Log($"GreenGoblin {gameObject.name} enabled!");
     }
    */

}

public abstract class BaseDeath : State { }
public abstract class BaseWalk : State { }
public abstract class BaseStun : State { }
public abstract class BaseIdel : State { }
public abstract class BaseAttack : State { }