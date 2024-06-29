using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T _instance;
    protected static bool _applicationIsQuitting = false;

    public static T Instance
    {
        get
        {
            if (_applicationIsQuitting)
            {
                return null;
            }

            if (_instance == null)
            {
                _instance = (T)FindObjectOfType(typeof(T));
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<T>();
                    singletonObject.name = "(singleton) " + typeof(T).ToString();
                    DontDestroyOnLoad(singletonObject);
                }
                else
                {
                    // Ensure the existing instance is at the root level
                    if (_instance.gameObject.transform.parent != null)
                    {
                        _instance.gameObject.transform.parent = null;
                        DontDestroyOnLoad(_instance.gameObject);
                    }
                }
            }

            return _instance;
        }
    }

    protected virtual void OnApplicationQuit()
    {
        _applicationIsQuitting = true;
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
           
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
}