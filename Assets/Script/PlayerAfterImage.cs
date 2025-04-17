using System.Collections;
using UnityEngine;

public class PlayerAfterImage : APoolingObject
{
    public SpriteRenderer sr;
    public float fadeSpeed;
    public float lifeTime;
    
    public override void OnBirth()
    {
        sr.sprite = null;
        gameObject.transform.localScale = new Vector3(1, 1, 1);
        sr.color = new Color(0, 0, 0, 255);
    }

    public override void OnDeathInit()
    {
        OnBirth();
    }

    public void SetIamge(Sprite sprite, float alpha, float size, float flipX)
    {
        sr.sprite = sprite;
        sr.color = new Color(0, 0, 0, alpha);
        gameObject.transform.localScale = new Vector3(flipX, size, 1);
        StartCoroutine(FadeFlow());

    }
    private IEnumerator FadeFlow()
    {
        var a = 2f;
        for(var t = 0f; t <= lifeTime; t += Time.deltaTime)
        {
            a = Mathf.Lerp(a, 0f, fadeSpeed * Time.deltaTime);
            sr.color = new Color(0, 0, 0,a);
            yield return null;
        }
        ObjectPooler.Instance.Return(this);
    }

}
