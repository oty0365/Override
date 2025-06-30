using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class KeyBinder
{
    public string keyName;
    public TextMeshProUGUI keyText;
    public Button button;
}

public class KeyBindingManager : SingleMono<KeyBindingManager>
{
    public List<KeyBinder> keyBinders;
    public Dictionary<string, KeyBinder> keyBindDict = new();
    public Dictionary<string, KeyCode> keyBindings = new();
    public bool isSelectingKey;
    [SerializeField] private GameObject dualKeyPannel;
    private string currentSelectingKey;


    protected override void Awake()
    {
        base.Awake();
        keyBindings["Up"] = KeyCode.W;
        keyBindings["Down"] = KeyCode.S;
        keyBindings["Left"] = KeyCode.A;
        keyBindings["Right"] = KeyCode.D;
        keyBindings["Attack1"] = KeyCode.Mouse0;
        keyBindings["Attack2"] = KeyCode.Mouse1;
        keyBindings["SpecialAttack1"] = KeyCode.Q;
        keyBindings["SpecialAttack2"] = KeyCode.E;
        keyBindings["Ultimate"] = KeyCode.R;
        keyBindings["Dash"] = KeyCode.Space;
        keyBindings["Interact"] = KeyCode.F;
        keyBindings["Charge"] = KeyCode.LeftControl;
    }
    void Start()
    {
        dualKeyPannel.SetActive(false);

        foreach (var i in keyBinders)
        {
            keyBindDict.Add(i.keyName, i);
            string keyName = i.keyName;
            var button = i.button;
            button.onClick.AddListener(() =>
            {
                WaitToSetKey(keyName);
            });
        }

        var keys = keyBindings.Keys.ToList();
        for (int i = 0; i < keys.Count; i++)
        {
            string key = keys[i];

            if (PlayerPrefs.HasKey(key))
            {
                keyBindings[key] = (KeyCode)PlayerPrefs.GetInt(key);
            }
        }
        UpdateAllKeys();
    }
    public void UpdateAllKeys()
    {
        foreach (var i in keyBindDict)
        {
            i.Value.keyText.text = keyBindings[i.Key].ToString();
        }
        SaveKeyBindSets();
    }
    public void UpdateKey(string keyCode)
    {
         keyBindings[keyCode] = (KeyCode)PlayerPrefs.GetInt(keyCode);
         keyBindDict[keyCode].keyText.text = keyBindings[keyCode].ToString();
         SaveKeyBindSets();
    }
    void Update()
    {
        if (isSelectingKey && Input.anyKeyDown)
        {
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                {
                    PlayerPrefs.SetInt(currentSelectingKey, (int)key);
                    UpdateKey(currentSelectingKey);
                    currentSelectingKey = "";
                    isSelectingKey = false;
                }

            }
        }
    }
    public void WaitToSetKey(string keyCode)
    {
        currentSelectingKey = keyCode;
        isSelectingKey = true;
        foreach (var i in keyBinders)
        {
            if (i.keyName == currentSelectingKey)
            {
                i.keyText.text = "...";
            }
        }
    }
    public void SaveKeyBindSets()
    {
        foreach (var i in keyBindings)
        {
            keyBindDict[i.Key].keyText.color = Color.white;
            foreach (var j in keyBindings)
            {
                keyBindDict[i.Key].keyText.color = Color.white;
                if (i.Key != j.Key && i.Value == j.Value)
                {
                    DualKeyException(i.Key,j.Key);
                    return;
                }
            }
        }
    }
    public void DualKeyException(string Key1, string Key2)
    {
        keyBindDict[Key1].keyText.color = Color.red;
        keyBindDict[Key2].keyText.color = Color.red;
        if (MainMenuManager.Instance.optionPannel.activeSelf)
        {
            dualKeyPannel.SetActive(true);
        }
    }
    public void Confrim()
    {
        dualKeyPannel.SetActive(false);
    }
}
