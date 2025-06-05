using UnityEngine;

public class ThrownSlikSkill : ACharacterSkill
{
    public InstantinateModule instantinateModule;
    private PlayerInfo _playerInfo;
    public LayerMask layerMask;
    public void Start()
    {
        _playerInfo = PlayerInfo.Instance;
        UpdateSkill();
    }
    public void Update()
    {
        instantinateModule.attackTransform.position = (Vector2)PlayerMove.Instance.gameObject.transform.position + WeaponCore.Instance.mouseDir.normalized * instantinateModule.range;
        if (Input.GetKeyDown(_currentKey)&&SkillManager.Instance.CheckToUseSkill(skillForm))
        {
            UseSkill();
        }
    }
    public override void UseSkill()
    {
        var dir = WeaponCore.Instance.mouseDir;
        var o =ObjectPooler.Instance.Get(instantinateModule.attackObj,instantinateModule.attackTransform.position, new Vector3(0, 0, WeaponCore.Instance.rotaion-90));
        RaycastHit2D hit = Physics2D.Raycast(_playerInfo.transform.position, dir, Mathf.Infinity,layerMask);
        if (hit.collider != null)
        {
            o.GetComponent<ThrownSlik>().target = hit.point - dir * 0.1f;
        }


        SkillManager.Instance.StartSkillCooldown(skillForm);
    }

}
