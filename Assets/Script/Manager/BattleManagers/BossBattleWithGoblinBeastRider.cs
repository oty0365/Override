using System.Collections;
using UnityEngine;

public class BossBattleWithGoblinBeastRider : BossBattleManager
{
    [SerializeField]
    private string bossBgmCode;

    void Start()
    {
        Debug.Log("=== BossBattle Start 호출 ===");
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

        Debug.Log("=== 11단계: 보스 타겟을 플레이어로 설정 및 카메라 복원 ===");
        boss.target = player;
        PlayerCamera.Instance.target = player;
        playerCam.transform.position = player.transform.position;

        Debug.Log("=== 12단계: 카메라 줌 설정 ===");
        PlayerCamera.Instance.currentZoomSize = 6;
        PlayerCamera.Instance.SetZoom(6, 14f);

        Debug.Log("=== 13단계: SetBossBattle 호출 ===");
        SetBossBattle();

        Debug.Log("=== StartBattleFlow 완료 ===");
    }
}