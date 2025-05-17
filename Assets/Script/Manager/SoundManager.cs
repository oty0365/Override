using UnityEngine;
using UnityEngine.UI;

public class SoundManager : SingleMono<SoundManager>
{ 
    [SerializeField] private Slider volume;
    public float SoundVolume
    {
        get => volume.value;
        set
        {
            volume.value = value;
        }
    }
    void Start()
    {
        volume.maxValue = 100f;
        if (!PlayerPrefs.HasKey("Volume"))
        {
            PlayerPrefs.SetFloat("Volume", 50f);
        }
        SoundVolume = PlayerPrefs.GetFloat("Volume");

        volume.onValueChanged.AddListener((float value) => { SoundVolume = value; OnSaveSounds(); });
    }

    public void OnSaveSounds()
    {
        PlayerPrefs.SetFloat("Volume", SoundVolume);
    }
}
