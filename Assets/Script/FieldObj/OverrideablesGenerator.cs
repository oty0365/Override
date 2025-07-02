using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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
        var currentPlayerCharacterData = PlayerInfo.Instance.currentCharacterData;
        var availableCharacters = new List<OverrideablesData>();

        foreach (var character in overrideables.overrideablesList)
        {
            if (currentPlayerCharacterData == null ||character != currentPlayerCharacterData)
            {
                availableCharacters.Add(character);
            }
        }
        if (availableCharacters.Count == 0)
        {
            availableCharacters = overrideables.overrideablesList.ToList();
        }

        var randomIndex = Random.Range(0, availableCharacters.Count);
        var selectedCharacter = availableCharacters[randomIndex];

        var spawnedObject = Instantiate(selectedCharacter.prefabObject,spawnPos.transform.position,Quaternion.identity); 

        return spawnedObject;
    }
    public GameObject RandomSpawnExcluding(OverrideablesData excludeCharacter)
    {
        var availableCharacters = overrideables.overrideablesList.Where(character => character != excludeCharacter).ToArray();

        if (availableCharacters.Length == 0)
        {
            Debug.LogWarning("제외할 캐릭터를 빼면 사용 가능한 캐릭터가 없습니다.");
            return RandomSpawn(); 
        }

        var randomIndex = Random.Range(0, availableCharacters.Length);
        return Instantiate(availableCharacters[randomIndex].prefabObject,spawnPos.transform.position,Quaternion.identity);
    }
}
