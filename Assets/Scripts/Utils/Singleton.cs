using UnityEngine;

namespace GGJ2026.Core.Utils
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField]
        protected bool _dontDestroyOnLoad = false;

        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.LogWarning($"[Singleton<{typeof(T).Name}>] Instance is null. Creating one but instead make sure the singleton is present in the scene.");
                    SetInstance();
                }

                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;

                if (_dontDestroyOnLoad)
                    DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            // Always clear the static reference if this is the current instance
            if (_instance == this)
            {
                _instance = null;
            }
        }

        private static void SetInstance()
        {
            _instance = FindFirstObjectByType<T>();

            if (_instance == null)
            {
                GameObject singletonObject = new GameObject(typeof(T).Name);
                _instance = singletonObject.AddComponent<T>();
            }
        }
    }
}
