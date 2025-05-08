using System.Collections;
using UnityEngine;

public class RunesOfAugment : AInteractable
{
    public override void OnInteract()
    {
        Time.timeScale = 0;
        GameEventManager.Instance.eventsDict[InGameEvent.AugmentOrItemSelection].Invoke();
        StartCoroutine(DestructionFlow());
    }
    private IEnumerator DestructionFlow()
    {
        yield return new WaitForSecondsRealtime(0.4f);
        Destroy(gameObject);
    }
}
