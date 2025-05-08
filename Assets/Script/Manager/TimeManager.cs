using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance {get;private set;}
    public float feildTime = 1f;

    private void Awake()
    {
        Instance = this;
    }
}
