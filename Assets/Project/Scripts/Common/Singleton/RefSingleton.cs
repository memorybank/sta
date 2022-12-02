using System;
using System.Collections;
using UnityEngine;
using System.Reflection;

public class RefSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    private static object _lock = new object();

    public static T Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null || _instance.gameObject == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        Debug.LogError("[Singleton] Something went really wrong " +
                            " - there should never be more than 1 singleton!" +
                                       " Reopenning the scene might fix it." + "\n");
                        return _instance;
                    }

                    if (_instance != null)
                    {
                        // -- Add to stack for destory when game restart
                        SingletonStack.AddSingleton(_instance);

                        Debug.Log("[Singleton] Using instance already created: " +
                                  _instance.gameObject.name + "\n");
                    }
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
    }

    /**
     * used when restart game
     */
    public void DestorySingleton()
    {
        //        if (_instance != null)
        //        {
        //            Debug.Log("RefSingleton.DestorySingleton Instance: " + _instance.GetType());
        //        }

        _instance = null;
    }
}
