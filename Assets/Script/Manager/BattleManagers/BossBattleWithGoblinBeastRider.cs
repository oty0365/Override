using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;

public class BossBattleWithGoblinBeastRider : BossBattleManager
{
    void Start()
    {
        InitBoss();
        //boss.target = null;
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
        Debug.Log("Done");
        yield return new WaitForSeconds(0.9f);

        PlayerInteraction.Instance.OnInteractMode(1);
        boss.target = player;
        PlayerCamera.Instance.target = player;
        playerCam.transform.position = player.transform.position;
        PlayerCamera.Instance.SetZoom(6, 14f);
        SetBossBattle();
    }

}

