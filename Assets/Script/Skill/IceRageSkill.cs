using UnityEngine;

public class IceRageSkill : ACharacterSkill
{
    public InstantinateModule instantinateModule;
    public void Start()
    {
        UpdateSkill();
    }
    public void Update()
    {
        instantinateModule.attackTransform.position = (Vector2)PlayerMove.Instance.gameObject.transform.position+WeaponCore.Instance.mouseDir.normalized * instantinateModule.range;
        if (Input.GetKeyDown(_currentKey)&&SkillManager.Instance.CheckToUseSkill(skillForm))
        {
            UseSkill();
        }
    }
    public override void UseSkill()
    {
        var o =ObjectPooler.Instance.Get(instantinateModule.attackObj, instantinateModule.attackTransform.position, new Vector3(0, 0, WeaponCore.Instance.rotaion));
        var cb = o.gameObject.GetComponent<IceRage>();
        var sr = o.gameObject.GetComponent<SpriteRenderer>();
        if (WeaponCore.Instance.mouseDir.x > 0)
        {
            sr.flipY = false;
        }
        else
        {
            sr.flipY = true;
        }
            cb.curDir = WeaponCore.Instance.mouseDir.normalized * instantinateModule.range;
        SkillManager.Instance.StartSkillCooldown(skillForm);
    }

}
