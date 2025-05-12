using System.Collections;
using UnityEngine;

public enum StareMode
{
    Battom_Up,
    Up_Bottom,
    Both
}


public class StareCutscene : AInteractable
{
    public GameObject starePoint;
    public float waitTime;
    public StareMode stareMode;
    void Start()
    {
        
    }
    void Update()
    {
        
    }
    public override void OnInteract()
    {
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
        PlayerCamera.Instance.target = starePoint;
        yield return new WaitForSeconds(waitTime);
        PlayerCamera.Instance.target = PlayerInfo.Instance.gameObject;
        PlayerInteraction.Instance.OnInteractMode(1);
    }
    public override void ExitInteract()
    {
    }

}
