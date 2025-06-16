using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BossBattleWithGoblinBeastRider : BossBattleManager
{
    void Start()
    {
        InitBoss();
        boss.target = null;
        StartCoroutine(StartBattleFlow());
    }
    private IEnumerator StartBattleFlow()
    {
        var player = PlayerInfo.Instance.gameObject;
        var playerCam = PlayerCamera.Instance.gameObject;
        PlayerInteraction.Instance.OnInteractMode(0);
        PlayerCamera.Instance.target = boss.gameObject;
        while (Vector2.Distance(playerCam.transform.position, boss.transform.position) > 0.4f)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.9f);
        boss.target = player;
        yield return new WaitForSeconds(1f);
        PlayerInteraction.Instance.OnInteractMode(1);
        boss.target = player;
        PlayerCamera.Instance.target = player;
        playerCam.transform.position = player.transform.position;
        PlayerCamera.Instance.currentZoomSize = 6;
        PlayerCamera.Instance.SetZoom(6, 14f);
        SetBossBattle();
    }

}

