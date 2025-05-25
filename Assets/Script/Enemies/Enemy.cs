using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public abstract class Enemy : APoolingObject
{
    public MonsterData monsterData;
    public Transform staminaPointPos;
    public Animator ani;
    public SpriteRenderer sr;
    public Action onStaminaChange;
    public Collider2D enemyHitBox;
    protected MaterialPropertyBlock _metProps;
    protected Coroutine _currentHitFlow;
    public Dictionary<Collider2D, bool> contactWithDamage = new(); 
    protected Coroutine _currentInfinateFlow;

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

    public virtual void InitEnemy()
    {
        contactWithDamage.Clear();
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
            contactWithDamage.Remove(other);
        }
    }

    private void FixedUpdate()
    {
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
}
