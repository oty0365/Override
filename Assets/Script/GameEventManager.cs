using UnityEngine;
using UnityEngine.Playables;

public class GameEventManager : MonoBehaviour
{
    public static GameEventManager Instance { get; private set; }
    private int _playMode;
    public int PlayMode { get => _playMode; private set=> _playMode = value; }
    public int currentEventIndex;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (PlayerPrefs.HasKey("EventIdx"))
        {
            currentEventIndex = PlayerPrefs.GetInt("EventIdx");
        }
        else
        {
            PlayerPrefs.SetInt("EventIdx", 0);
            currentEventIndex = PlayerPrefs.GetInt("EventIdx");
        }
    }
    public void CheckPlayMode()
    {
        
    }
    public void SetPlayMode(int mode)
    {
        PlayMode = mode;
    }
  
}
