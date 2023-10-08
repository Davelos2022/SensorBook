using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    private static object lockObject = new object();
    private static bool destroyed = false;

    public static bool HasInstance => instance != null;

    public static T Instance
    {
        get
        {
            //if (destroyed) return null;

            lock (lockObject)
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();

                    if (instance == null)
                    {
                        var singleton = new GameObject("[SINGLETON] " + typeof(T));
                        instance = singleton.AddComponent<T>();
                    }
                }
                return instance;
            }
        }
    }

    public static void DontDestroy()
    {
        DontDestroyOnLoad(Instance.gameObject);
    }

    private void OnDestroy()
    {
        destroyed = true;
    }
}