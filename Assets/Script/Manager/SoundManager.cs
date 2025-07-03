using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : SingleMono<SoundManager>
{ 
    [SerializeField] private Slider volume;
    public AudioSource bgm;
    public APoolingObject sfx;
    public float fadeSpeed = 1f;
    private Dictionary<string,AudioClip> soundDict = new();
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
        AudioClip[] clips = Resources.LoadAll<AudioClip>("SFX");
        foreach (var clip in clips)
        {
            soundDict.Add(clip.name, clip);
        }
        foreach(var i in clips)
        {
            Debug.Log(i);
        }
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
    public void PlaySFX(string key)
    {
        AudioSource source = GetAvailableSource();
        source.clip = soundDict[key];
        source.Play();
    }

    private AudioSource GetAvailableSource()
    {
        return ObjectPooler.Instance.Get(sfx, gameObject.transform.position, new Vector3(0, 0, 0)).GetComponent<AudioSource>();

    }
    public void PlayBGM(string key)
    {
        StartCoroutine(FadeToNewBGM(key));
    }

    private IEnumerator FadeToNewBGM(string key)
    {
        /*if (bgm != null)
        {
            while (bgm.volume > 0)
            {
                bgm.volume -= Time.deltaTime * fadeSpeed;
                yield return null;
            }
        }*/
        yield return null;
        if (soundDict.ContainsKey(key))
        {
            bgm.clip = soundDict[key];
            bgm.volume = 1;
            bgm.Play();
        }

        /*
        while (bgm.volume < 1)
        {
            bgm.volume += Time.deltaTime * fadeSpeed;
            yield return null;
        }*/
    }
    private void OnDestroy()
    {
        Debug.Log(1);
    }
}
