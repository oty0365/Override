using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

[Flags]
public enum EnableSkills
{
    None = 0,
    Identity = 1<<0,
    AtSkill = 1<<1,
    Ultimate = 1<<2
}

[System.Serializable]
public class SkillSlot
{
    public Image skillIcon;
    public Image skillFrame;
    public TextMeshProUGUI skillKey;
    public Image skillCooldownImage;
    public TextMeshProUGUI skillCooldown;
    public Coroutine skillCoolCoroutine;
}

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance { get; private set; }
    public EnableSkills enableSkills;
    [Header("스킬 정보")]
    public SkillSlot[] skillSlot;
    [Header("스킬 리스트")]
    public List<ACharacterSkill> characterSkillList;
    public SkillData curridentitySkill;
    public SkillData currultimateSkill;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        enableSkills |= EnableSkills.Identity;
        enableSkills |= EnableSkills.AtSkill;
        enableSkills |= EnableSkills.Ultimate;

        for(int i = 0; i < 3; i++)
        {
            CheckSkill(i);
        }

        ChangeCharacterSkill();
        ChangeAtSkill();
        SetInputKey();
    }

    public void SetInputKey()
    {
        skillSlot[0].skillKey.text = KeyBindingManager.Instance.keyBindings["SpecialAttack1"].ToString();
        skillSlot[1].skillKey.text = KeyBindingManager.Instance.keyBindings["SpecialAttack2"].ToString();
        skillSlot[2].skillKey.text  = KeyBindingManager.Instance.keyBindings["Ultimate"].ToString();
    }
    public void BreakSkillCooldown(int index)
    {
        if (skillSlot[index].skillCoolCoroutine != null)
        {
            StopCoroutine(skillSlot[index].skillCoolCoroutine);
            skillSlot[index].skillCooldown.text = "";
            skillSlot[index].skillCooldownImage.fillAmount = 0f;
            skillSlot[index].skillCooldownImage.gameObject.SetActive(false);
            switch (index)
            {
                case 0:
                    enableSkills |= EnableSkills.Identity;
                    break;
                case 2:
                    enableSkills |= EnableSkills.Ultimate;
                    break;
            }
        }
    }

    public void ChangeCharacterSkill()
    {

        foreach (var i in characterSkillList)
        {
            Destroy(i.gameObject);
        }
        characterSkillList.Clear();
        var s =PlayerInfo.Instance.currentSkillData;
        if (s == null)
        {
            skillSlot[0].skillIcon.color = Color.black;
            skillSlot[2].skillIcon.color = Color.black;
            skillSlot[0].skillFrame.color = Color.gray;
            skillSlot[2].skillFrame.color = Color.gray;


        }
        else
        {
            skillSlot[0].skillIcon.color = Color.white;
            skillSlot[2].skillIcon.color = Color.white;
            skillSlot[0].skillFrame.color = Color.white;
            skillSlot[2].skillFrame.color = Color.white;

            curridentitySkill = s.identitySkills[0].skillData;
            if (s.identitySkills[0].characterSkill != null)
            {
                InstantinateSkillManager(s.identitySkills[0]);
            }
            currultimateSkill = s.ultimateSkills[0].skillData;
            if (s.ultimateSkills[0].characterSkill != null)
            {
                InstantinateSkillManager(s.ultimateSkills[0]);
            }
            if (s.identitySkills[0].characterSkill != null)
            {
                Updatekey(s.identitySkills[0].characterSkill.skillForm);
            }
            if (s.ultimateSkills[0].characterSkill != null)
            {
                Updatekey(s.ultimateSkills[0].characterSkill.skillForm);
            }

            skillSlot[0].skillIcon.sprite = curridentitySkill.skillIcon;
            skillSlot[2].skillIcon.sprite = currultimateSkill.skillIcon;
        }
    }
    public void InstantinateSkillManager(SkillSets skillSet)
    {
        var o = Instantiate(skillSet.characterSkill, gameObject.transform);
        characterSkillList.Add(o);
    }

    public void ChangeAtSkill()
    {
        skillSlot[1].skillIcon.color = Color.black;
        skillSlot[1].skillFrame.color = Color.gray;
        //atSkillIcon.color = Color.white;
        //atSkillFrame.color = Color.white;
    }

    public void Updatekey(SkillForm skillForm)
    {
        switch (skillForm)
        {
            case SkillForm.PassiveIdentity:
                skillSlot[0].skillKey.text = "";
                break;
            case SkillForm.PassiveAt:
                skillSlot[1].skillKey.text = "";
                break;
            case SkillForm.PassiveUltimate:
                skillSlot[2].skillKey.text = "";
                break;
            default:
                SetInputKey();
                break;

        }
    }

    public bool CheckToUseSkill(SkillForm skillForm)
    {
        bool value = false;
        switch (skillForm)
        {
            case SkillForm.PassiveIdentity:
            case SkillForm.ActiveIdentity:
                if ((enableSkills &= EnableSkills.Identity) != 0)
                {
                    value = true;
                }
                break;
            case SkillForm.PassiveAt:
            case SkillForm.ActiveAt:
                if ((enableSkills &= EnableSkills.AtSkill) != 0)
                {
                    value = true;
                }
                break;
            case SkillForm.PassiveUltimate:
            case SkillForm.ActiveUltimate:
                if ((enableSkills &= EnableSkills.Ultimate) != 0)
                {
                    value = true;
                }
                break;
        }
        return value;
    }

    public void StartSkillCooldown(SkillForm skillForm)
    {
        switch (skillForm)
        {
            case SkillForm.PassiveIdentity:
            case SkillForm.ActiveIdentity:                
                    enableSkills &= ~EnableSkills.Identity;
                    CheckSkill(0);
                    skillSlot[0].skillCoolCoroutine = StartCoroutine(SkillCoolFlow(0));
                
                break;
            case SkillForm.PassiveAt:
            case SkillForm.ActiveAt:
                    enableSkills &= ~EnableSkills.AtSkill;
                    CheckSkill(1);
                    skillSlot[1].skillCoolCoroutine = StartCoroutine(SkillCoolFlow(1));
                
                break;
            case SkillForm.PassiveUltimate:
            case SkillForm.ActiveUltimate:
                enableSkills &= ~EnableSkills.Ultimate;
                CheckSkill(2);
                skillSlot[2].skillCoolCoroutine = StartCoroutine(SkillCoolFlow(2));
                break;
        }
    }
    private IEnumerator SkillCoolFlow(int index)
    {
        float waitTime = 0f;
        var curObj = skillSlot[index].skillCooldownImage.gameObject;
        EnableSkills currSkillType = EnableSkills.Identity;
        switch (index)
        {
            case 0:
                waitTime = curridentitySkill.basicCoolDown;
                currSkillType = EnableSkills.Identity;
                break;
            case 2:
                waitTime = currultimateSkill.basicCoolDown;
                currSkillType = EnableSkills.Ultimate;
                break;
        }

        float timer = 0f;
        curObj.SetActive(true);
        while (timer < waitTime)
        {
            timer += Time.deltaTime;
            skillSlot[index].skillCooldown.text = Mathf.CeilToInt(waitTime - timer).ToString();
            skillSlot[index].skillCooldownImage.fillAmount = Mathf.Clamp01(1f - (timer / waitTime));
            yield return null;
        }
        skillSlot[index].skillCooldown.text = "";
        skillSlot[index].skillCooldownImage.fillAmount = 0f;
        curObj.SetActive(false);
        enableSkills |= currSkillType;
    }

    public void CheckSkill(int skillIndex)
    {
        if((enableSkills & EnableSkills.Identity) != 0)
        {
            skillSlot[skillIndex].skillCooldownImage.gameObject.SetActive(false);
        }
        else
        {
            skillSlot[skillIndex].skillCooldownImage.gameObject.SetActive(false);
            skillSlot[skillIndex].skillCooldown.text = curridentitySkill.basicCoolDown.ToString();
        }
    }
}
