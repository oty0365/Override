using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public enum Language 
{
    Ko,
    En
}

public class Script
{
    public string key;
    public string type;
    public int priority;
    public string ko;
    public string en;
    public string currentText;
}

public class Scripter : SingleMono<Scripter>
{
    public Language curLanguage;
    [SerializeField] private ScriptingArray array;
    public Dictionary<string, Script> scripts = new();

    protected override void Awake()
    {
        base.Awake();
        LoadAllCsv();
    }

    public void LoadAllCsv()
    {
        foreach (var i in array.scriptingArray)
        {
            TextAsset csv = Resources.Load<TextAsset>(i);
            if (csv == null)
            {
                Debug.LogError($"CSV 파일 {i} 을(를) 찾을 수 없습니다.");
                return;
            }
            string[] lines = csv.text.Split('\n');
            bool isFirstLine = true;

            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line))
                    continue;

                if (isFirstLine)
                {
                    isFirstLine = false;
                    continue;
                }

                string[] values = line.Split(',');
                if (values.Length < 5)
                {
                    continue;
                }
                string key = values[0].Trim();
                string typeStr = values[1].Trim();
                string priorityStr = values[2].Trim();
                string ko = values[3].Trim().Replace("{COMMA}",",");
                string en = values[4].Trim().Replace("{COMMA}", ",");

                if (!int.TryParse(priorityStr, out int priority))
                {
                    continue;
                }

                if (!scripts.ContainsKey(key))
                {
                    var script = new Script()
                    {
                        key = key,
                        type = typeStr,
                        priority = priority,
                        ko = ko,
                        en = en
                    };
                    scripts.Add(key, script);
                }
                switch (curLanguage)
                {
                    case Language.Ko:
                        scripts[key].currentText = ko;
                        break;
                    case Language.En:
                        scripts[key].currentText = en;
                        break;
                }
            }
        }
    }
}
