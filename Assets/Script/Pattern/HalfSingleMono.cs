using UnityEngine;

public abstract class HalfSingleMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    private static object _lock = new();

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                lock (_lock)
                {
                    instance = FindAnyObjectByType<T>();
                    if (instance == null)
                    {
                        var singletonObject = new GameObject(typeof(T).Name);
                        instance = singletonObject.AddComponent<T>();
                    }
                }
            }
            return instance;
        }
    }
    protected virtual void Awake()
    {
        instance = this as T;
    }
}

