using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
	public static T instance { get; private set;}

	public virtual void Awake()
	{
		if (instance == null)
		{
			instance = this as T;
            Debug.Log(instance.name);
		}
		else
		{
			Destroy(gameObject);
		}
	}
}
