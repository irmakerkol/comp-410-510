using UnityEngine;

public class EventHandler<T> : MonoBehaviour where T : MonoBehaviour
{
    static T _instance;
    public static T instance
    {
        get
        {
            if (_instance == null)
                _instance = new GameObject("EventHandler" + nameof(T)).AddComponent<T>();

            return _instance;
        }
    }

}
