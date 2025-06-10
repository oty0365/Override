using System.Collections;
using UnityEngine;

public class StaminaPoint : APoolingObject
{
    public Enemy currentEnemy;
    public SpriteRenderer sr;
    [SerializeField] private Gradient gradient;
    public float progress;
    private Coroutine _waitFlow;
    private Coroutine _recuveryFlow;




    void Start()
    {
    }
    void Update()
    {
        if (currentEnemy != null && currentEnemy.staminaPointPos != null)
            transform.position = currentEnemy.staminaPointPos.position;
    }
    public override void OnBirth()
    {
        progress = 0f;
        sr.color = gradient.Evaluate(progress);

    }
    public void OnvalueChange()
    {
        progress = (currentEnemy.monsterData.maxStamina - currentEnemy.CurrentStamina) / currentEnemy.monsterData.maxStamina;
        if (progress >= 1)
        {
            //currentEnemy.CurrentStamina = currentEnemy.monsterData.maxStamina;
            currentEnemy.isStun = true;
        }
        else
        {
            sr.color = gradient.Evaluate(progress);
        }
        if (_waitFlow != null)
        {
            StopCoroutine(_waitFlow);
        }
        _waitFlow = StartCoroutine(WaitFlow());

    }
    public override void OnDeathInit()
    {
    }
    private IEnumerator WaitFlow()
    {
        yield return new WaitForSeconds(currentEnemy.monsterData.recoveryTime);

        if (!isActiveAndEnabled || currentEnemy == null)
            yield break;

        if (_recuveryFlow != null)
            StopCoroutine(_recuveryFlow);

        _recuveryFlow = StartCoroutine(RecoveryFlow());
    }


    private IEnumerator RecoveryFlow()
    {
        float maxStamina = currentEnemy.monsterData.maxStamina;

        while (progress > 0)
        {
            if (!isActiveAndEnabled || currentEnemy == null || sr == null)
                yield break;

            progress -= Time.deltaTime * 3f;
            progress = Mathf.Clamp01(progress);

            sr.color = gradient.Evaluate(progress);

            currentEnemy.CurrentStamina = maxStamina * (1f - progress);

            yield return null;
        }

        progress = 0f;
        currentEnemy.CurrentStamina = maxStamina;
    }
}