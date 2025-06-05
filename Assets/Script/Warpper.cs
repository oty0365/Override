using System.Collections;
using UnityEngine;

public class Warpper : AInteractable
{
    [SerializeField] private Collider2D hitBox;
    [SerializeField] private bool isGoingToRoot;
    void Start()
    {
        
    }
    public override void OnInteract()
    {
        hitBox.enabled = false;
        PlayerInfo.Instance.gameObject.transform.position = gameObject.transform.position;
        PlayerInteraction.Instance.OnInteractMode(0);
        StartCoroutine(InteractionFlow());
    }
    private IEnumerator InteractionFlow()
    {
        MapManager.Instance.CurrentDimention = Dimention.Code;
        PlayerCamera.Instance.gameObject.transform.position = PlayerInfo.Instance.transform.position;
        PlayerCamera.Instance.SetZoom(1f,8.3f);
        yield return new WaitForSeconds(1.2f);
        if (isGoingToRoot)
        {
            MapManager.Instance.RootNodeMap();
        }
        else
        {
            MapManager.Instance.NextMap();
        }

        MapManager.Instance.CurrentDimention = Dimention.Normal;
        PlayerCamera.Instance.SetZoom(4.5f, 15f);
        PlayerInteraction.Instance.OnInteractMode(1);
    }
    public override void ExitInteract()
    {

    }
}
