using UnityEngine;
public abstract class BossBattleManager : MonoBehaviour
{
    public Enemy boss;
    [SerializeField] private string bossName;

    public virtual void InitBoss()
    {
        boss.InitEnemy();
    }
    public virtual void SetBossBattle()
    {
        
        var bossBar = UIManager.Instance.bossBar;
        bossBar.bossSlider.gameObject.SetActive(true);
        bossBar.bossNameText.text = Scripter.Instance.scripts[bossName].currentText;
        bossBar.bossSlider.maxValue = boss.CurrentHp;
        ShowHp();
        boss.onHitAction += ShowHp;
        boss.onDeathAction += EndBossBattle;
        

    }
    public virtual void ShowHp()
    {
        UIManager.Instance.bossBar.bossSlider.value = boss.CurrentHp;
    }

    public virtual void EndBossBattle()
    {
        PlayerCamera.Instance.currentZoomSize = 4.5f;
        var bossBar = UIManager.Instance.bossBar;
        bossBar.bossSlider.gameObject.SetActive(false);
    }
}
