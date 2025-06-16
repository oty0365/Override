using UnityEngine;

public class SFXObj : APoolingObject
{
    public AudioSource currentSource;
    void Start()
    {
        
    }

    void Update()
    {
        if (!currentSource.isPlaying)
        {
            Death();
        }
    }
    public override void OnBirth()
    {
        
    }
    public override void OnDeathInit()
    {
        
    }
}
