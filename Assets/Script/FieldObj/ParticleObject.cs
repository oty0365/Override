using System.Collections;
using UnityEngine;

public class ParticleObject : APoolingObject
{
    public ParticleSystem prt;
    private float waitTime;


    public override void OnDeathInit()
    {

    }
    public override void OnBirth()
    {
        waitTime = prt.main.duration;
        StartCoroutine(ParticleFlow());
    }
    private IEnumerator ParticleFlow()
    {
        prt.Play();
        yield return new WaitForSeconds(waitTime);
        Death();
    }
}
