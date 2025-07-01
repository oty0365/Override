using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct BossBar
{
    public Slider bossSlider;
    public TextMeshProUGUI bossNameText;
}

public class UIManager : HalfSingleMono<UIManager>
{
    [Header("º¸½º ¹Ù")]
    public BossBar bossBar;

    private void Start()
    {
        bossBar.bossSlider.gameObject.SetActive(false);

    }
}
