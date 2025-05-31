using System.Collections;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;

public class RiseOfTheRadiance :APoolingObject
{
    private PlayerInfo playerInfo;
    public float infinateTime;
    public RiseOfTheRadianceSkill manager;
    [SerializeField] private SpriteRenderer sr;
    public float fadeSpeed;
    void Start()
    {
        
    }

    void Update()
    {
        gameObject.transform.position = new Vector2(playerInfo.gameObject.transform.position.x, playerInfo.gameObject.transform.position.y+0.3f);
    }
    public override void OnBirth()
    {
        sr.color = Color.clear;
        playerInfo = PlayerInfo.Instance;
        StartCoroutine(FadeFlow());
    }
    private IEnumerator FadeFlow()
    {
        for(var i = 0f; i <= 1f; i += Time.deltaTime)
        {
            sr.color = Color.Lerp(sr.color, Color.white, fadeSpeed * Time.deltaTime);
            yield return null;
        }
        sr.color = Color.white;
        playerInfo.SetInfiniteTime(infinateTime);
        var counter = 0f;
        for(var i = 0f; i < infinateTime; i += Time.deltaTime)
        {
            if (i > counter)
            {
                counter+=0.5f;
                playerInfo.PlayerCurStamina+=3;
                playerInfo.PlayerAttackDamage += 20;
            }
            yield return null;
        }
        for (var i = 0f; i <= 1.5f; i += Time.deltaTime)
        {
            sr.color = Color.Lerp(sr.color, Color.clear, fadeSpeed * Time.deltaTime);
            yield return null;
        }
        sr.color = Color.clear;
        playerInfo.PlayerAttackDamage -= 20;
    }
    public override void OnDeathInit()
    {
        manager.currentShield = null;
        manager = null;
    }
}
