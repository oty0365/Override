using UnityEngine;
using UnityEngine.AddressableAssets;


public class TitleUI : MonoBehaviour
{
    private void Start()
    {
        OnAble();
    }
    public void OnAble()
    {
        Time.timeScale = 0;
    }
    public void OnStart()
    {
        Time.timeScale = 1;
        //Addressables.ReleaseUnusedAssets();
        gameObject.SetActive(false);
    }
    public void OnQuit()
    {
        Application.Quit();
    }
}
