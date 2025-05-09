using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class AugmentUI
{
    public TextMeshProUGUI augmentUIName;
    public Image augmentUIImage;
    public TextMeshProUGUI augmentUIDesc;
}

public class AugmentManager : MonoBehaviour
{
    public static AugmentManager Instance { get; private set; }
    public AugmentDatas augmentDatas;
    private List<AugmentData> _augmentDatas = new List<AugmentData>();
    public AugmentData[] randomDatas = new AugmentData[3];
    public AugmentUI[] augmentUIs = new AugmentUI[3]; 


    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        CheckAndUploadAugments();
    }
    void Update()
    {
        
    }

    public void ActiveAugment(int index)
    {
        foreach(var i in randomDatas[index].effect)
        {
            GameEventManager.Instance.eventsDict[i].Invoke();
        }

    }

    public void RandomAugmentOutput()
    {
        _augmentDatas.Clear();
        _augmentDatas.AddRange(augmentDatas.augments);
        var range = _augmentDatas.Count;
        for(var i = 0; i < 3; i++)
        {
            var index =UnityEngine.Random.Range(0, range);
            randomDatas[i] = _augmentDatas[index];
            augmentUIs[i].augmentUIImage.sprite = randomDatas[i].augmentSprite;
            augmentUIs[i].augmentUIName.text = randomDatas[i].augmentName;
            augmentUIs[i].augmentUIDesc.text = randomDatas[i].augmentDesc;
            _augmentDatas.Remove(randomDatas[i]);
            range--;
        }

    }

    public void CheckAndUploadAugments()
    {
        var methods = GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        foreach (var method in methods)
        {
            var attribute = method.GetCustomAttribute<EventUploadAttribute>();
            if (attribute != null)
            {
                if (Enum.TryParse<InGameEvent>(method.Name, out var inGameEvent))
                {
                    if (method.GetParameters().Length == 0 && method.ReturnType == typeof(void))
                    {
                        var action = (Action)Delegate.CreateDelegate(typeof(Action), this, method);
                        GameEventManager.Instance.UploadEvent(inGameEvent, action);
                    }
                    else
                    {
                        UnityEngine.Debug.LogWarning($"{method.Name}은 매개변수가 없고 void 반환형이어야 합니다.");
                    }
                }
            }
        }

        //GameEventManager.Instance.PrintEvnets();
    }


    [EventUpload()]
    public void HpUp()
    {
        PlayerInfo.Instance.playerMaxHp += 15;
        PlayerInfo.Instance.PlayerCurHp += 15;
    }
    [EventUpload]
    public void FullHp()
    {
        PlayerInfo.Instance.PlayerCurHp = PlayerInfo.Instance.playerMaxHp;
    }
    [EventUpload]
    public void CoolTimeUp()
    {
        PlayerInfo.Instance.playerBasicSkillCooldown += 0.15f;
    }
    [EventUpload]
    public void AttackUp()
    {
        PlayerInfo.Instance.playerBasicAttackDamage += 15;
    }
}
