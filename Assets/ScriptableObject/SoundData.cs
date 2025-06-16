using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SoundData
{
    public string key;
    public AudioClip clip;
    public float volume = 1.0f;
}

[CreateAssetMenu(fileName = "SoundLibrary", menuName = "Audio/SoundLibrary")]
public class SoundLibrary : ScriptableObject
{
    public List<SoundData> sounds;

    public SoundData GetSound(string key) => sounds.Find(s => s.key == key);
}