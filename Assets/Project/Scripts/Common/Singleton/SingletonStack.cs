using System;
using System.Collections;
using UnityEngine;
using System.Reflection;

public class SingletonStack
{
    public static ArrayList ListSingleton = new ArrayList();

    /**
     * 重启时需要等到loading level加载后, 才能destory, 比如player, 不然update()函数会报错.
     */
    public static ArrayList ListSingletonKeep = new ArrayList();

    public static void AddSingleton(object stSingleton)
    {
        if (stSingleton.GetType().ToString().Equals("Player"))
        {
            //Debug.Log("AddSingleton Instance: " + stSingleton.GetType() + ", add to ListSingletonKeep");
            if (ListSingletonKeep.IndexOf(stSingleton) == -1)
            {
                ListSingletonKeep.Add(stSingleton);
            }
        }
        else
        {
            if (ListSingleton.IndexOf(stSingleton) == -1)
            {
                ListSingleton.Add(stSingleton);
            }
        }
    }

    public static void DestoryKeepSingleton()
    {
        string debugstr = "";
        for (int i = ListSingletonKeep.Count - 1; i >= 0; i--)
        {
            object st = ListSingletonKeep[i];
            debugstr += st.ToString() + ", ";
            Type boundedType = st.GetType();
            MethodInfo m = boundedType.GetMethod("DestorySingleton", BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            if (m != null)
            {
                m.Invoke(st, null);
            }
        }
        ListSingletonKeep.Clear();
        Debug.Log("DestoryAllSingleton Destory list is: " + debugstr);
    }

    public static void DestoryLoadingSceneSingleton()
    {
        string debugstr = "";
        for (int i = ListSingleton.Count - 1; i >= 0; i--)
        {
            object st = ListSingleton[i];
            debugstr += st.ToString() + ", ";
            Type boundedType = st.GetType();
            MethodInfo m = boundedType.GetMethod("DestorySingleton", BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            if (m != null)
            {
                m.Invoke(st, null);
            }
        }
        Debug.Log("DestoryLoadingSceneSingleton Destory list is: " + debugstr);
    }
}
