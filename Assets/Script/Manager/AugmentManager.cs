using System;
using Unity.VisualScripting;
using UnityEngine;

public class AugmentManager : MonoBehaviour
{
    public static AugmentManager Instance { get; private set; }

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

    public void CheckAndUploadAugments()
    {
        var methods = GetType().GetMethods();
        foreach(var method in methods)
        {
            var attributeSlot = method.GetAttribute<EventUploadAttribute>();
            if (attributeSlot != null)
            {
                if (Enum.TryParse<InGameEvent>(method.Name, out var inGameEvent))
                {
                    GameEventManager.Instance.UploadEvent(inGameEvent, attributeSlot.gameAction);
                }
                GameEventManager.Instance.UploadEvent(inGameEvent, attributeSlot.gameAction);
            }
        }
    }

    [EventUpload]
    public void HpUp()
    {
        
    }
    [EventUpload]
    public void FullHp()
    {

    }
    [EventUpload]
    public void CoolTimeUp()
    {

    }
    [EventUpload]
    public void AttackUp()
    {

    }
}
