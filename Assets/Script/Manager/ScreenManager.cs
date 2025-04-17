using UnityEngine;
using UnityEngine.UI;

public class ScreenManager : MonoBehaviour
{
    public CanvasScaler scaler;
    private Vector2 defaultResolution = new Vector2(1920, 1080);
    private float lastScreenWidth;
    private float lastScreenHeight;
    void Start()
    {
        AdjustCanvasResolution();
    }
    void AdjustCanvasResolution()
    {
        float screenRatio = (float)Screen.width / Screen.height;
        float referenceRatio = defaultResolution.x / defaultResolution.y;

        if (screenRatio > referenceRatio)
        {
            scaler.referenceResolution = new Vector2(defaultResolution.y * screenRatio, defaultResolution.y);
        }
        else
        {
            scaler.referenceResolution = new Vector2(defaultResolution.x, defaultResolution.x / screenRatio);
        }
    }
    void Update()
    {
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            AdjustCanvasResolution();
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
        }
    }
}
