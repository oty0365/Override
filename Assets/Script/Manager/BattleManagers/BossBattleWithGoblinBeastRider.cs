using System.Collections;
using UnityEngine;

public class BossBattleWithGoblinBeastRider : BossBattleManager
{
    [SerializeField]
    private string bossBgmCode;

    void Start()
    {
        Debug.Log("=== BossBattle Start ȣ�� ===");
        InitBoss();
        boss.target = null;
        PlayerCamera.Instance.transform.position = new Vector3(boss.gameObject.transform.position.x,boss.gameObject.transform.position.y, PlayerCamera.Instance.transform.position.z-5f);
        PlayerCamera.Instance.target = boss.gameObject;
        if (bossBgmCode != null)
        {
            SoundManager.Instance.PlayBGM(bossBgmCode);
        }

        StartCoroutine(StartBattleFlow());
    }

    private IEnumerator StartBattleFlow()
    {
        var player = PlayerInfo.Instance.gameObject;
        var playerCam = PlayerCamera.Instance.gameObject;

        PlayerInteraction.Instance.OnInteractMode(0);
        Debug.Log(PlayerCamera.Instance.target);
        for(var i = 0f; i <= 2.5f; i += Time.deltaTime)
        {
            PlayerCamera.Instance.transform.position = new Vector3(boss.gameObject.transform.position.x, boss.gameObject.transform.position.y, PlayerCamera.Instance.transform.position.z);
            PlayerCamera.Instance.target = boss.gameObject;
            yield return null;
        }
        //yield return new WaitForSeconds(4f);

        PlayerInteraction.Instance.OnInteractMode(1);

        Debug.Log("=== 11�ܰ�: ���� Ÿ���� �÷��̾�� ���� �� ī�޶� ���� ===");
        boss.target = player;
        PlayerCamera.Instance.target = player;
        playerCam.transform.position = player.transform.position;

        Debug.Log("=== 12�ܰ�: ī�޶� �� ���� ===");
        PlayerCamera.Instance.currentZoomSize = 6;
        PlayerCamera.Instance.SetZoom(6, 14f);

        Debug.Log("=== 13�ܰ�: SetBossBattle ȣ�� ===");
        SetBossBattle();

        Debug.Log("=== StartBattleFlow �Ϸ� ===");
    }
}