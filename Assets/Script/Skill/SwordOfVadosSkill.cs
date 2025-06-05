using UnityEngine;

public class SwordOfVadosSkill : ACharacterSkill
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
        var o =ObjectPooler.Instance.Get(instantinateModule.attackObj, instantinateModule.attackTransform.position, new Vector3(0, 0, WeaponCore.Instance.rotaion-90));
        var cb = o.gameObject.GetComponent<SwordOfVados>();
        cb.curDir = WeaponCore.Instance.mouseDir.normalized * instantinateModule.range;
        cb.curRotation = WeaponCore.Instance.rotaion - 90;
        SkillManager.Instance.StartSkillCooldown(skillForm);
    }

}
