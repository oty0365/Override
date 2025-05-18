using System.Collections;
using UnityEngine;

public class Warpper : AInteractable
{
    public MapData nextMap;
    [SerializeField] private Collider2D hitBox;

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
        PlayerCamera.Instance.SetZoom(2f,15f);
        yield return new WaitForSeconds(1);
        MapManager.Instance.CurrentDimention = Dimention.Normal;
        PlayerCamera.Instance.SetZoom(4.5f, 20f);
        PlayerInteraction.Instance.OnInteractMode(1);
    }
    public override void ExitInteract()
    {

    }
}
