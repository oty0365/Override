using UnityEngine;

public class GodSlimesBleastSkill : ACharacterSkill
{
    public InstantinateModule instantinateModule;
    private APoolingObject aPoolingObject;
    public APoolingObject currentShield;
    private PlayerInfo playerInfo;
    public void Start()
    {
        aPoolingObject = instantinateModule.attackObj.GetComponent<APoolingObject>();
        playerInfo = PlayerInfo.Instance;
        UpdateSkill();
    }
    public void Update()
    {
        if (currentShield == null && SkillManager.Instance.CheckToUseSkill(skillForm)&& Input.GetKeyDown(_currentKey))
        {
            UseSkill();
        }
    }
    public override void UseSkill()
    {
        var o = ObjectPooler.Instance.Get(aPoolingObject, PlayerInfo.Instance.gameObject.transform.position, new Vector3(0, 0, 0));
        o.GetComponent<GodSlimesBleast>().manager = this;
        currentShield = o.GetComponent<APoolingObject>();
        SkillManager.Instance.StartSkillCooldown(skillForm);
    }
    private void OnDestroy()
    {
        Destroy(currentShield.gameObject);

    }
}
