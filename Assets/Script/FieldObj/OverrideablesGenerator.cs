using UnityEngine;

public class OverrideablesGenerator : MonoBehaviour
{
    public OverrideablesArray overrideables;
    public GameObject spawnPos;
    public AtTutorialBot tutorialBot;
    void Start()
    {
        if (tutorialBot != null)
        {
            tutorialBot.makeInteracter = RandomSpawn();
            tutorialBot.makeInteracter.layer = LayerMask.NameToLayer("Default");
        }
        else
        {
            RandomSpawn();
        }
    }
    public GameObject RandomSpawn()
    {
        var index = Random.Range(0, overrideables.overrideablesList.Length);
        var o =Instantiate(overrideables.overrideablesList[index].prefabObject, spawnPos.transform.position, Quaternion.identity);
        return o;
    }
}
