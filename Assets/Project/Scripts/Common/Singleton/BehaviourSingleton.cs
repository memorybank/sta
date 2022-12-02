using UnityEngine;

/// <summary>
/// Be aware this will not prevent a non singleton constructor
///   such as `T myT = new T();`
/// To prevent that, add `protected T () {}` to your singleton class.
/// 
/// As a note, this is made as MonoBehaviour because we need Coroutines.
/// </summary>
public class BehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static bool NeedDestroyOnRestart = false;

    private static object _lock = new object();

    public static T Instance
    {
        get
        {
            //            if (applicationIsQuitting)
            //            {
            //                Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
            //                    "' already destroyed on application quit." +
            //				                 " Won't create again - returning null." + "\n");
            //                return null;
            //            }

            lock (_lock)
            {
                if (_instance == null || _instance.gameObject == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        Debug.LogError("[Singleton] Something went really wrong " +
                                       " - there should never be more than 1 singleton! [" + typeof(T).ToString() +
                                       "] Reopenning the scene might fix it." + "\n");
                        _instance.gameObject.GetComponent<RectTransform>();
                        DontDestroyOnLoad(_instance.gameObject);
                        return _instance;
                    }

                    if (_instance == null)
                    {
                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = "(singleton) " + typeof(T).ToString();
                        //Debug.Log("[Singleton] An instance of " + typeof (T).ToString() + " is needed in the scene, so '" + singleton.name + "' was created with DontDestroyOnLoad.\n");
                    }
                    else
                    {
                        Debug.Log("[Singleton] Using instance already created: " +
                                  _instance.gameObject.name + "\n");
                    }
                    DontDestroyOnLoad(_instance.gameObject);
                    SingletonStack.AddSingleton(_instance);  // -- Add to stack for destory when game restart
                }

                return _instance;
            }
        }
    }

    private static bool applicationIsQuitting = false;
    /// <summary>
    /// When Unity quits, it destroys objects in a random order.
    /// In principle, a Singleton is only destroyed when application quits.
    /// If any script calls Instance after it have been destroyed, 
    ///   it will create a buggy ghost object that will stay on the Editor scene
    ///   even after stopping playing the Application. Really bad!
    /// So, this was made to be sure we're not creating that buggy ghost object.
    /// </summary>
    public virtual void OnDestroy()
    {
        applicationIsQuitting = true;
#if !SERVER
        //SDKConfig.RemoteDebugRemove(gameObject);
#endif
    }

    public virtual void Awake()
    {
#if !SERVER
        //SDKConfig.RemoteDebugAdd(gameObject);
#endif
    }

    /**
     * used when restart game
     */
    public void DestorySingleton()
    {
        if (_instance != null)
        {
            Debug.Log("BehaviourSingleton.DestorySingleton Instance: " + _instance.GetType());
            if (NeedDestroyOnRestart)
            {
                if (_instance.gameObject != null)
                {
                    Debug.Log("BehaviourSingleton.DestorySingleton destroy gameobject Instance: " + _instance.GetType());
                    DestroyImmediate(_instance.gameObject);
                }
            }
        }
        _instance = null;
    }
}
