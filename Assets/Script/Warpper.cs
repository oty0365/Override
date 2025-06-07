using System.Collections;
using UnityEngine;

public enum WarpMode
{
    Root,
    Next,
    Game
}
public class Warpper : AInteractable
{
    [SerializeField] private Collider2D hitBox;
    [SerializeField] private WarpMode warpMode;
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
        switch (warpMode)
        {
            case WarpMode.Root:
                MapManager.Instance.RootNodeMap();
                break;
            case WarpMode.Next:
                MapManager.Instance.NextMap();
                break;
            case WarpMode.Game:
                MapManager.Instance.SetGameMap(3,9,10,11);
                break;
        }
        MapManager.Instance.CurrentDimention = Dimention.Normal;
        PlayerCamera.Instance.SetZoom(4.5f, 15f);
        PlayerInteraction.Instance.OnInteractMode(1);
    }
    public override void ExitInteract()
    {

    }
}
