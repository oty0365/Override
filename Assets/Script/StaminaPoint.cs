using UnityEngine;

public class StaminaPoint : APoolingObject
{
    public Enemy currentEnemy;
    public SpriteRenderer sr;
    [SerializeField] private Gradient gradient;
    public float progress;




    void Start()
    {
    }
    void Update()
    {
        gameObject.transform.position = currentEnemy.transform.position;
    }
    public override void OnBirth()
    {
        progress = 0f;
        sr.color = gradient.Evaluate(progress);
        currentEnemy.onStaminaChange += OnvalueChange;


    }
    private void OnvalueChange()
    {
        progress = currentEnemy.CurrentStamina / currentEnemy.monsterData.maxStamina;
        sr.color = gradient.Evaluate(progress);
    }
    public override void OnDeathInit()
    {
        currentEnemy.onStaminaChange -= OnvalueChange;
    }
}
