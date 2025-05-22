using System.Collections;
using TMPro;
using UnityEngine;

public class Code : APoolingObject
{
    public string[] frase;
    public Color[] textColors;
    public TextMeshPro codeTmp;
    public Vector2 target;
    private bool isReturning;
    private Coroutine _liveFlow;
    public override void OnBirth()
    {
        isReturning = false;
        SkillManager.Instance.openCodes.Add(this);
        var r = Random.Range(0, frase.Length);
        var c = Random.Range(0, textColors.Length);
        codeTmp.text = frase[r];
        codeTmp.color = Color.white;
        _liveFlow = StartCoroutine(LiveFlow());
    }
    public void FallowPlayer()
    {
        //codeTmp.color = Color.black;
        //var n = Random.Range(0, 2);
        //codeTmp.text =n.ToString();
        isReturning = true;
        StopCoroutine(_liveFlow);
    }
    private IEnumerator LiveFlow()
    {
        yield return new WaitForSeconds(7f);
        Death();
    }
    private void Update()
    {
        gameObject.transform.position = Vector2.MoveTowards(gameObject.transform.position, target , 9 * Time.deltaTime);
        if (isReturning&&Vector2.Distance(gameObject.transform.position, PlayerInfo.Instance.gameObject.transform.position) < 0.05f)
        {
            Death();
        }
    }
    public override void OnDeathInit()
    {
        SkillManager.Instance.openCodes.Remove(this);
    }
}
