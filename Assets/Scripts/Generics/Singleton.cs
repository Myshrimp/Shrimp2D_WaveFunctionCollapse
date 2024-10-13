using UnityEngine;

namespace Shrimp2DWFC.Generics
{
    public class Singleton<T> :MonoBehaviour where T:Singleton<T>
    {
        private static T _instance;
        private static object _lock = new object();
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance = FindObjectOfType<T>();
                        if (_instance == null)
                        {
                            GameObject go = new GameObject(typeof(T).Name);
                            _instance = go.AddComponent<T>();
                        }
                    }
                }
                return _instance;
            }
        }
        
        
        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = (T)this;
                DontDestroyOnLoad(gameObject); //切换场景不销毁对象
            }
            else
            {
                Destroy(gameObject);
            }
        }
    
        protected virtual void OnDestroy()
        {
            _instance = null;
        }
    
        private void OnApplicationQuit()
        {
            _instance = null;
        }
    }
}