
public class Singleton<T> where T : new()
{
    public static string MsgTypeName = "";
    private static T instance;
    public static T Instance
    {
        get
        {
            if (Equals(instance, default(T)))
            {
                instance = new T();
                SingletonStack.AddSingleton(instance);
            }
            return instance;
        }
    }

    public void DestorySingleton()
    {
        //        if (instance != null)
        //        {
        //            Debug.Log("Singleton.DestorySingleton Instance: " + instance.GetType());
        //        }

        SingletonStack.ListSingleton.Remove(instance);
        instance = default(T);
    }

    public void InitSingleton()
    {
        if (Equals(instance, default(T)))
        {
            instance = new T();
            SingletonStack.AddSingleton(instance);
        }
    }
    
    /// <summary>
    /// 构造函数
    /// </summary>
    protected Singleton()
    {
        Initialization();
    }

    /// <summary>
    /// 初始化函数
    /// </summary>
    protected virtual void Initialization()
    {

    }
}
