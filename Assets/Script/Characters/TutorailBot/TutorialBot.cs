using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;

public class TutorialBot : AInteractable
{
    public string text;
    public TextMeshPro description;
    public float inputDuration;
    public float desolveDuration;
    public float exitTime;
    private Coroutine _currentCoroutine;
    private Scripter scripter;

    private void Start()
    {
        scripter = Scripter.Instance;
        description.text = "";
        text = scripter.scripts[text].currentText;
        var matches = Regex.Matches(text, @"<(\w+)>");
        foreach(Match match in matches)
        {
            string keyBind = match.Groups[1].Value;
            if (KeyBindingManager.Instance.keyBindings.ContainsKey(keyBind))
            {
                string displayKey = KeyBindingManager.Instance.keyBindings[keyBind].ToString();
                text = text.Replace($"<{keyBind}>", displayKey);
            }
        }
    }

    public override void OnInteract()
    {
        if(_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
        }
        description.text = "";
        description.gameObject.SetActive(true);
        if (!autoInteraction)
        {
            PlayerInteraction.Instance.OnInteractMode(0);
            _currentCoroutine = StartCoroutine(SetTextFlow());
        }
        else
        {
            _currentCoroutine = StartCoroutine(SetTextFlow());
        }
    }
    private IEnumerator SetTextFlow()
    {
        var newText = "";
        foreach (var i in text)
        {
            newText += i;
            description.text = newText;
            yield return new WaitForSeconds(inputDuration);
        }
        if (!autoInteraction)
        {
            yield return new WaitForSeconds(exitTime);
            description.gameObject.SetActive(false);
            PlayerInteraction.Instance.OnInteractMode(1);
        }
    }
    private IEnumerator RemoveTextFlow()
    {
        var newText = new Stack<string>(description.text.Select(c => c.ToString()));
        while (newText.Count > 0)
        {
            newText.Pop();
            description.text = new string(string.Join("", newText).Reverse().ToArray());
            yield return new WaitForSeconds(desolveDuration);
        }
    }
    public override void ExitInteract()
    {
        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
        }
        _currentCoroutine = StartCoroutine(RemoveTextFlow());
    }
}
