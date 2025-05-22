using System;
using System.Collections;
using UnityEngine;

public abstract class Enemy : APoolingObject
{
    public MonsterData monsterData;
    public Transform stamiaPointPos;
    public Animator ani;
    public SpriteRenderer sr;
    public Action onStaminaChange;
    protected MaterialPropertyBlock _metProps;
    protected Coroutine _currentHitFlow;

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
            if(value != _currentStamia)
            {
                _currentStamia = value;
                if(onStaminaChange != null)
                {
                    onStaminaChange.Invoke();
                }
            }
        }
    }
    public virtual void InitEnemy()
    {
        CurrentHp = monsterData.maxHp;
        CurrentStamina = monsterData.maxStamina;
        _metProps = new MaterialPropertyBlock();
        var a = ObjGenerator.Instance.generateDict["StaminaPoint"];
        var o = ObjectPooler.Instance.Get(a, stamiaPointPos.transform.position, new Vector3(0, 0, 45));
        var c = o.GetComponent<StaminaPoint>();
        c.currentEnemy = this;
        c.UpLoadEvent();
        
    }
    public virtual void Hit(float damage)
    {
        CurrentStamina -= 10f;
        if (_currentHitFlow != null)
        {
            StopCoroutine(_currentHitFlow);
        }
        _currentHitFlow = StartCoroutine(HitFlow());

    }

    private IEnumerator HitFlow()
    {
        for(var i = 0f; i < 1f; i += Time.deltaTime*8f)
        {
            sr.GetPropertyBlock(_metProps);
            _metProps.SetFloat("_Progress", i);
            sr.SetPropertyBlock(_metProps);
            yield return null;
        }
        sr.GetPropertyBlock(_metProps);
        _metProps.SetFloat("_Progress", 1);
        sr.SetPropertyBlock(_metProps);
        for (var i = 1f; i > 0f; i -= Time.deltaTime *8f)
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
        Debug.Log(other.tag);
        if (other.CompareTag("damageable"))
        {
            other.GetComponent<AAttack>().CastDamage(this);
        }
    }
}
