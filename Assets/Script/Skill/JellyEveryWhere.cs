using UnityEngine;

public class JellyEverWhereSkill : ACharacterSkill
{
    public InstantinateModule instantinateModule;
    private PlayerInfo playerInfo;

    public void Start()
    {
        playerInfo = PlayerInfo.Instance;
        UpdateSkill();
    }
    public void Update()
    {
        if (SkillManager.Instance.CheckToUseSkill(skillForm))
        {
            UseSkill();
        }
    }
    Vector2 GetRandomPointOnCircle(float radius, Vector2 center)
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float x = Mathf.Cos(angle) * radius + center.x;
        float y = Mathf.Sin(angle) * radius + center.y;
        return new Vector2(x, y);
    }

    public override void UseSkill()
    {
        var pos = GetRandomPointOnCircle(instantinateModule.range, playerInfo.gameObject.transform.position);
        var o = ObjectPooler.Instance.Get(instantinateModule.attackObj, pos, new Vector3(0, 0, 0));
        SkillManager.Instance.StartSkillCooldown(skillForm);
    }
}
