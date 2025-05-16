using UnityEngine;

public class MainMenuManager : SingleMono<MainMenuManager>
{
    public GameObject optionPannel;
    private void Start()
    {
        optionPannel.SetActive(false);
    }
    public void SetOptionPannel(bool willBeActived)
    {
        if (!willBeActived)
        {
            PlayerPrefs.Save();
        }
        optionPannel.SetActive(willBeActived);
    }
}
