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


    [SerializeField]
    private float _currentHp;
    public float CurrentHp
    {
        get => _currentHp;
        set
        {
            _currentHp = value;
            if (_currentHp <= 0)
            {
                Debug.Log("aa");
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
                if (value != _currentStamia)
                {
                    _currentStamia = value;
                    if (staminaPoint != null && staminaPoint.gameObject.activeInHierarchy)
                    {
                        staminaPoint.OnvalueChange();
                    }
                }
            }
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
            if(backParticle != null)
            {
                backParticle.SetActive(false);
            }

        }
        canAttack = true;
        isStun = false;
        isStunning = false;
        contactWithDamage.Clear();
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
    public void DeathDrop()
    {
        
        staminaPoint.Death();
        if(battleManager != null)
        {
            battleManager.MonsterCount--;
        }
        PlayerInfo.Instance.PlayerCoin += UnityEngine.Random.Range(monsterData.minCoin, monsterData.maxCoin + 1);
        if (isCurrupted)
        {
            backParticle.SetActive(false);
            glowEye.SetActive(false);
        }
    }

    public void RemovedByGame()
    {
        battleManager.MonsterCount--;
        staminaPoint.Death();
        Death();
    }

    public virtual void Hit(Collider2D collider, float damage, float infinateTime)
    {
        //Debug.Log("Hit");
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

            StartCoroutine(InfinateTimeFlow(collider, infinateTime));
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
        if(!contactWithDamage.ContainsKey(other) && other.CompareTag("damageable"))
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
    }

    protected virtual void FixedUpdate()
    {
        StateFixedUpdate();
        foreach (var kvp in contactWithDamage.ToList())
        {
            if (kvp.Key != null && !kvp.Value)
            {
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