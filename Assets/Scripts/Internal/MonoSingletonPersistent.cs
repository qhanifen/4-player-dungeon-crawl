using UnityEngine;

public class MonoSingletonPersistent<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T instance { get; private set; }

    public virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }        
    }
}
