using UnityEngine;

public class PlayerAfterImage : APoolingObject
{
    public SpriteRenderer sr;
    
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

    public void SetIamge(Sprite sprite, float alpha, float size)
    {
        sr.sprite = sprite;
        sr.color = new Color(0, 0, 0, alpha);
        gameObject.transform.localScale = new Vector3(size, size, 1);
    }

}
