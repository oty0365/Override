using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance { get; private set; }
    [Header("½ºÅ³ UI")]
    public Image identitySkillIcon;
    public Image identitySkillFrame;
    public TextMeshProUGUI identityKey;
    [Space(15)]
    public Image atSkillIcon;
    public Image atSkillFrame;
    public TextMeshProUGUI atKey;
    [Space(15)]
    public Image ultimateSkillIcon;
    public Image ultimateSkillFrame;
    public TextMeshProUGUI ultimateKey;

    public SkillData curridentitySkill;
    public SkillData currultimateSkill;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        ChangeCharacterSkill();
        ChangeAtSkill();
        ShowInputKey();
    }

    public void ShowInputKey()
    {
        identityKey.text = KeyBindingManager.Instance.keyBindings["SpecialAttack1"].ToString();
        atKey.text = KeyBindingManager.Instance.keyBindings["SpecialAttack2"].ToString();
        ultimateKey.text  = KeyBindingManager.Instance.keyBindings["Ultimate"].ToString();
    }

    public void ChangeCharacterSkill()
    {
        var s =PlayerInfo.Instance.currentSkillData;
        if (s == null)
        {
            identitySkillIcon.color = Color.black;
            ultimateSkillIcon.color = Color.black;
            identitySkillFrame.color = Color.gray;
            ultimateSkillFrame.color = Color.gray;


        }
        else
        {
            identitySkillIcon.color = Color.white;
            ultimateSkillIcon.color = Color.white;
            identitySkillFrame.color = Color.white;
            ultimateSkillFrame.color = Color.white;

            curridentitySkill = s.identitySkills[0].skillData;
            currultimateSkill = s.ultimateSkills[0].skillData;
            identitySkillIcon.sprite = curridentitySkill.skillIcon;
            ultimateSkillIcon.sprite = currultimateSkill.skillIcon;
        }
    }

    public void ChangeAtSkill()
    {
        atSkillIcon.color = Color.black;
        atSkillFrame.color = Color.gray;
        //atSkillIcon.color = Color.white;
        //atSkillFrame.color = Color.white;
    }
}
