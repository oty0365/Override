using TMPro;
using UnityEngine;

public class GameOverPanel : MonoBehaviour
{
    public GameObject gameOverPannel;
    public TextMeshProUGUI faildText;
    public TextMeshProUGUI dieMsgText;
    public TextMeshProUGUI gotoHome;
    private void Start()
    {
        gameOverPannel.SetActive(false);
    }

    public void GameOver()
    {
        gameOverPannel.SetActive(true);
        faildText.text = Scripter.Instance.scripts["YouDied"].currentText;
        var index = Random.Range(1, 8);
        dieMsgText.text = Scripter.Instance.scripts["DieMsg-" + index.ToString()].currentText;
        gotoHome.text = Scripter.Instance.scripts["PressToRespawn"].currentText;
    }

    public void OnHover()
    {
        gotoHome.color = Color.green;
    }
    public void ExitHover()
    {
        gotoHome.color = Color.white;
    }
    public void Enter()
    {
        PlayerCamera.Instance.SetAbHpUi(0);
        PlayerCamera.Instance.SetZoom(4.5f, 30f);
        PlayerCamera.Instance.currentZoomSize = 4.5f;
        gameOverPannel.SetActive(false);
        if (MapManager.Instance.battleManager != null)
        {
            MapManager.Instance.battleManager.ReturnAll();
        }
        UIManager.Instance.bossBar.bossSlider.gameObject.SetActive(false);
        MapManager.Instance.CurrentMonsters = 0;
        ObjectPooler.Instance.InitPoollist();
        MapManager.Instance.RootNodeMap();
    }
}
