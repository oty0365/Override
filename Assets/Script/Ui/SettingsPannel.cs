using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPannel : MonoBehaviour
{
    public GameObject settings;
    public bool isActived;
    public GameObject[] pannels;
    public int pannelMode;

    [SerializeField] private GameObject namePannel;

    [Header("스킬 정보")]
    [SerializeField] private Image player;
    [SerializeField] private Image skill1;
    [SerializeField] private Image skill2;
    [SerializeField] private Image skill3;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private GameObject skillDesc;
    [SerializeField] private TextMeshProUGUI skillDescText;
    [Header("플레이어 정보")]
    [SerializeField] private TextMeshProUGUI atk;
    [SerializeField] private TextMeshProUGUI def;
    [SerializeField] private TextMeshProUGUI hp;
    [SerializeField] private TextMeshProUGUI playerDesc;
    void Start()
    {
        pannelMode = 0;
        settings.SetActive(false);
        isActived = false;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)&&!MapManager.Instance.isLoading)
        {
            if (!isActived)
            {
                settings.SetActive(true);
                SetInfo();
                Time.timeScale = 0;
            }
            else
            {
                settings.SetActive(false);
                Time.timeScale = 1;
            }
            isActived = !isActived;
        }
    }
    public void OnInfoClicked()
    {
        if (pannelMode == 1)
        {
            pannels[pannelMode].SetActive(false);
            SetInfo();
            pannelMode = 0;
        }
        else
        {
            pannelMode = 1;
            pannels[pannelMode].SetActive(true);
        }
    }
    public void SetInfo()
    {
        if (SkillManager.Instance.curridentitySkill != null)
        {
            namePannel.SetActive(true);
            skill1.sprite = SkillManager.Instance.curridentitySkill.skillIcon;
            ClickSkillTree(1);
        }
        else
        {
            namePannel.SetActive(false);
        }
        if (SkillManager.Instance.currultimateSkill != null)
        {
            skill3.sprite = SkillManager.Instance.currultimateSkill.skillIcon;
        }


    }

    public void ClickSkillTree(int mode)
    {
        skillDesc.SetActive(true);
        switch (mode)
        {
            case 1:
                nameText.text = Scripter.Instance.scripts[ SkillManager.Instance.curridentitySkill.skillName].currentText;
                skillDescText.text = Scripter.Instance.scripts[SkillManager.Instance.curridentitySkill.skillDesc].currentText;
                break;
            case 3:
                nameText.text = Scripter.Instance.scripts[SkillManager.Instance.currultimateSkill.skillName].currentText;
                skillDescText.text = Scripter.Instance.scripts[SkillManager.Instance.currultimateSkill.skillDesc].currentText;
                break;
        }
    }
}
