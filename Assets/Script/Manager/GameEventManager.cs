using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public enum InGameEvent
{
    None,
    WeaponSelection,
    AugmentOrItemSelection,
    FullHp,
    HpUp,
    CoolTimeUp,
    AttackUp,
    AtDown,
    SelectDemon,
    SelectAdventureWeapon
}

public class GameEventManager : HalfSingleMono<GameEventManager>
{

    public Dictionary<InGameEvent,Action> eventsDict = new Dictionary<InGameEvent,Action>();

    private int _playMode;
    public int PlayMode { get => _playMode; private set=> _playMode = value; }
    public int currentStoryIndex;

    void Start()
    {
        if (PlayerPrefs.HasKey("StoryIdx"))
        {
            currentStoryIndex = PlayerPrefs.GetInt("StoryIdx");
        }
        else
        {
            PlayerPrefs.SetInt("StoryIdx", 0);
            currentStoryIndex = PlayerPrefs.GetInt("StoryIdx");
        }
    }
    public void CheckPlayMode()
    {
        
    }
    public void SetPlayMode(int mode)
    {
        PlayMode = mode;
    }

    public void UploadEvent(InGameEvent inGameEvent,Action action)
    {
        if (eventsDict.ContainsKey(inGameEvent)) 
        {
            eventsDict.Remove(inGameEvent);
        }
        eventsDict.Add(inGameEvent, action);

    }
    public void PrintEvnets()
    {
        foreach(var i in eventsDict)
        {
            Debug.Log(i);
        }
    }
  
}
