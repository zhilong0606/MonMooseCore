using System.IO;
using UnityEngine;

namespace MonMoose.Core
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        [SerializeField]
        private bool m_dontDestroy;

        private static T m_instance;

        public static T instance
        {
            get
            {
                CreateInstance();
                return m_instance;
            }
        }

        public static bool hasInstance
        {
            get { return m_instance != null; }
        }

        public static void CreateInstance(bool dontDestroy = false)
        {
            CreateInstanceInternal(null, dontDestroy);
        }

        public static void CreateInstanceByAsset(string path, bool dontDestroy = false)
        {
            GameObject go = null;
            if (m_instance == null)
            {
                if (!string.IsNullOrEmpty(path))
                {
                    GameObject prefab = ResourceManager.instance.LoadSync<GameObject>(path);
                    if (prefab != null)
                    {
                        go = Instantiate(prefab);
                    }
                }
                if (go == null)
                {
                    go = new GameObject(typeof(T).Name);
                }
            }
            CreateInstanceInternal(go, dontDestroy);
        }

        public static void CreateInstanceByResources(string path, bool dontDestroy = false)
        {
            GameObject go = null;
            if (m_instance == null)
            {
                if (!string.IsNullOrEmpty(path))
                {
                    GameObject prefab = Resources.Load<GameObject>(path);
                    if (prefab != null)
                    {
                        go = Instantiate(prefab);
                    }
                }
                if (go == null)
                {
                    go = new GameObject(typeof(T).Name);
                }
            }
            CreateInstanceInternal(go, dontDestroy);
        }

        public static void DestroyInstance()
        {
            if (m_instance != null)
            {
                Destroy(m_instance);
            }
        }

        private static void CreateInstanceInternal(GameObject go, bool dontDestroy = false)
        {
            if (m_instance != null)
            {
                return;
            }
            if (go == null)
            {
                go = new GameObject(typeof(T).Name);
            }
            m_instance = go.GetComponent<T>();
            if (m_instance == null)
            {
                m_instance = go.AddComponent<T>();
                if (dontDestroy)
                {
                    DontDestroyOnLoad(go);
                }
            }
            else
            {
                if (m_instance.m_dontDestroy)
                {
                    DontDestroyOnLoad(go);
                }
            }
            m_instance.OnInit();
        }

        void Awake()
        {
            if (m_instance == null)
            {
                CreateInstanceInternal(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void OnDestroy()
        {
            if (m_instance != null)
            {
                OnUnInit();
                m_instance = null;
            }
        }

        protected virtual void OnInit()
        {

        }

        protected virtual void OnUnInit()
        {

        }
    }
}