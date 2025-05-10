using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class TutorialBot : AInteractable
{
    [TextArea]public string text;
    public TextMeshPro description;
    public float inputDuration;
    private Coroutine _currentCoroutine;

    private void Start()
    {
        description.text = "";
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
        _currentCoroutine = StartCoroutine(SetTextFlow());

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
    }
    private IEnumerator RemoveTextFlow()
    {
        var newText = new Stack<string>(description.text.Select(c => c.ToString()));
        while (newText.Count > 0)
        {
            newText.Pop();
            description.text = new string(string.Join("", newText).Reverse().ToArray());
            yield return new WaitForSeconds(inputDuration);
        }
    }
    public override void ExitInteract()
    {
        StopCoroutine(_currentCoroutine);
        _currentCoroutine = StartCoroutine(RemoveTextFlow());
    }
}
