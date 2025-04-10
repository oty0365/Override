using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance { get; private set; }
    public GameObject optionPannel;

    private void Awake()
    {
        Instance = this;
    }
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
