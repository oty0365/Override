using System.Collections;
using UnityEngine;

public class PlayerAfterImage : APoolingObject
{
    public SpriteRenderer sr;
    public float fadeSpeed;
    public float lifeTime;
    private float _currentColor;

    public override void OnBirth()
    {
        _currentColor = 0;
        sr.sprite = null;
        gameObject.transform.localScale = new Vector3(1, 1, 1);
        sr.color = new Color(_currentColor, _currentColor, _currentColor, 255);
    }

    public override void OnDeathInit()
    {
        OnBirth();
    }

    public void SetIamge(Sprite sprite, float alpha, float size, float flipX)
    {
        sr.sprite = sprite;
        sr.color = new Color(_currentColor, _currentColor, _currentColor, alpha);
        gameObject.transform.localScale = new Vector3(flipX, size, 1);
        StartCoroutine(FadeFlow());

    }
    private IEnumerator FadeFlow()
    {
        var a = 2f;
        for(var t = 0f; t <= lifeTime; t += Time.deltaTime)
        {
            a = Mathf.Lerp(a, 0f, fadeSpeed * Time.deltaTime);
            sr.color = new Color(_currentColor, _currentColor, _currentColor,a);
            yield return null;
        }
        ObjectPooler.Instance.Return(this);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("hitable"))
        {
            Debug.Log("!");
            _currentColor = 1;
        }
    }
}
