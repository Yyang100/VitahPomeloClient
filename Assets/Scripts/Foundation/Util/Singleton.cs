using System;
using System.Reflection;

public class Singleton<T> where T : class
{
    /*  Instance  */
    private static T instance;

    /* Serve the single instance to callers */
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (T)Activator.CreateInstance(typeof(T), true);
                Type type = typeof(T);
                MethodInfo mi = type.GetMethod("OnInit");
                if (mi != null)
                {
                    mi.Invoke(instance, null);
                }
            }

            return instance;
        }
    }

    /*  Destroy */
    public static void Destroy()
    {
        Type type = typeof(T);
        MethodInfo mi = type.GetMethod("OnDestroy");
        if (mi != null)
        {
            mi.Invoke(instance, null);
        }

        instance = null;
        return;
    }
}
