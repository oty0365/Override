using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroFade : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(IntroFlow());
    }
    private IEnumerator IntroFlow()
    {
        yield return new WaitForSeconds(4.5f);
        SceneManager.LoadSceneAsync(1);
    }
}
