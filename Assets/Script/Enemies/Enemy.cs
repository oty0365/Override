using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public abstract class Enemy : APoolingObject
{
    [Header("몬스터 데이터")]
    public MonsterData monsterData;
    [Header("스테미나 포인트 위치")]
    public Transform staminaPointPos;
    [Header("몬스터 애니메이터")]
    public Animator ani;
    public SpriteRenderer sr;
    [Header("몬스터 물리연산")]
    public Action onStaminaChange;
    public Collider2D enemyHitBox;
    public Rigidbody2D rb2D;
    protected MaterialPropertyBlock _metProps;
    protected Coroutine _currentHitFlow;
    public FSM fsm = new();
    public Dictionary<Collider2D, bool> contactWithDamage = new(); 
    protected Coroutine _currentInfinateFlow;
    [Header("공격할 대상")]
    public GameObject target;

    [SerializeField]
    private float _currentHp;
    public float CurrentHp
    {
        get => _currentHp;
        set
        {
            _currentHp = value;
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
                if (onStaminaChange != null)
                {
                    onStaminaChange.Invoke();
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
        contactWithDamage.Clear();
        fsm.InitState();
        CurrentHp = monsterData.maxHp;
        CurrentStamina = monsterData.maxStamina;
        _metProps = new MaterialPropertyBlock();
        var a = ObjGenerator.Instance.generateDict["StaminaPoint"];
        var o = ObjectPooler.Instance.Get(a, staminaPointPos.transform.position, new Vector3(0, 0, 45));
        var c = o.GetComponent<StaminaPoint>();
        c.currentEnemy = this;
        c.UpLoadEvent();
    }
    

    public virtual void Hit(Collider2D collider, float damage, float infinateTime)
    {
        //Debug.Log("Hit");
        CurrentStamina -= 10f;
        if (_currentHitFlow != null)
        {
            StopCoroutine(_currentHitFlow);
        }
        _currentHitFlow = StartCoroutine(HitFlow());

        StartCoroutine(InfinateTimeFlow(collider, infinateTime));
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

    protected void FixedUpdate()
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
    protected void Update()
    {
        StateUpdate();
        Flip();
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
        if(target != null)
        {
            if (target.transform.position.x < gameObject.transform.position.x)
            {
                sr.flipX = true;
            }
            else
            {
                sr.flipX = false;
            }
        }

    }


}

public abstract class BaseDeath : State { }
public abstract class BaseWalk : State { }
public abstract class BaseStun : State { }
public abstract class BaseIdel : State { }
public abstract class BaseAttack : State { }