using System.Collections;
using UnityEngine;

public enum StareMode
{
    Battom_Up,
    Up_Bottom,
    Both
}
public enum PlayMode
{
    PlayOnce,
    Repeat
}


public class StareCutscene : AInteractable
{
    public GameObject starePoint;
    public float waitTime;
    public bool isPlayedOnce;
    public PlayMode playMode;
    public StareMode stareMode;
    void Start()
    {

    }
    void Update()
    {
        
    }
    public override void OnInteract()
    {
        switch (playMode)
        {
            case PlayMode.PlayOnce:
                if (isPlayedOnce)
                {
                    return;
                }
                break;
            case PlayMode.Repeat:
                break;
        }
        
        switch (stareMode)
        {
            case StareMode.Battom_Up:
                if (PlayerInfo.Instance.gameObject.transform.position.y < gameObject.transform.position.y)
                {
                    PlayerInteraction.Instance.OnInteractMode(0);
                    StartCoroutine(LookAtFlow());
                }
                break;
            case StareMode.Up_Bottom:
                if (PlayerInfo.Instance.gameObject.transform.position.y > gameObject.transform.position.y)
                {
                    PlayerInteraction.Instance.OnInteractMode(0);
                    StartCoroutine(LookAtFlow());
                }
                break;
            case StareMode.Both:
                PlayerInteraction.Instance.OnInteractMode(0);
                StartCoroutine(LookAtFlow());
                break;
        }

    }
    private IEnumerator LookAtFlow()
    {
        isPlayedOnce = true;
        PlayerCamera.Instance.target = starePoint;
        yield return new WaitForSeconds(waitTime);
        PlayerCamera.Instance.target = PlayerInfo.Instance.gameObject;
        PlayerInteraction.Instance.OnInteractMode(1);
    }
    public override void ExitInteract()
    {
    }

}
