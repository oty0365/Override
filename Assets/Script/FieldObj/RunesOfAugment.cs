using System.Collections;
using UnityEngine;

public class RunesOfAugment : AInteractable
{
    private void Start()
    {
        MapCleaner.Instance.Add(gameObject);
        /*if (gameObject.scene.IsValid())
        {
            //gameObject.transform.SetParent(MapManager.Instance.currentMap.mapPrefab.transform, true);
        }*/

    }
    public override void OnInteract()
    {
        AugmentManager.Instance.RandomAugmentOutput();
        Time.timeScale = 0;
        GameEventManager.Instance.eventsDict[InGameEvent.AugmentOrItemSelection].Invoke();
        StartCoroutine(DestructionFlow());
    }
    private IEnumerator DestructionFlow()
    {
        yield return new WaitForSecondsRealtime(0.4f);
        Destroy(gameObject);
    }
    public override void ExitInteract()
    {
        
    }
}
