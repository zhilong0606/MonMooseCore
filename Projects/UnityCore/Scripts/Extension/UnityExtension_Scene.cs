using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MonMoose.Core
{
    public static partial class UnityExtension
    {
        private static readonly List<GameObject> m_tempGameObjectList = new List<GameObject>();

        public static T GetComponent<T>(this UnityEngine.SceneManagement.Scene scene) where T : Component
        {
            if (!scene.IsValid())
            {
                return null;
            }
            scene.GetRootGameObjects(m_tempGameObjectList);
            T result = null;
            int count = m_tempGameObjectList.Count;
            for (int i = 0; i < count; ++i)
            {
                T component = m_tempGameObjectList[i].GetComponent<T>();
                if(component != null)
                {
                    result = component;
                    break;
                }
            }
            m_tempGameObjectList.Clear();
            return result;
        }

        public static T GetComponentInChildren<T>(this UnityEngine.SceneManagement.Scene scene, bool includeInactive = false) where T : Component
        {
            if (!scene.IsValid())
            {
                return null;
            }
            scene.GetRootGameObjects(m_tempGameObjectList);
            T result = null;
            int count = m_tempGameObjectList.Count;
            for (int i = 0; i < count; ++i)
            {
                T component = m_tempGameObjectList[i].GetComponentInChildren<T>(includeInactive);
                if (component != null)
                {
                    result = component;
                    break;
                }
            }
            m_tempGameObjectList.Clear();
            return result;
        }

        public static GameObject FindGameObject(this UnityEngine.SceneManagement.Scene scene, string name)
        {
            if (!scene.IsValid())
            {
                return null;
            }
            scene.GetRootGameObjects(m_tempGameObjectList);
            GameObject result = null;
            int count = m_tempGameObjectList.Count;
            for (int i = 0; i < count; ++i)
            {
                if (m_tempGameObjectList[i].name == name)
                {
                    result = m_tempGameObjectList[i];
                    break;
                }
            }
            m_tempGameObjectList.Clear();
            return result;
        }

        public static Transform FindTransform(this UnityEngine.SceneManagement.Scene scene, string name)
        {
            if (!scene.IsValid())
            {
                return null;
            }
            scene.GetRootGameObjects(m_tempGameObjectList);
            Transform result = null;
            int count = m_tempGameObjectList.Count;
            for (int i = 0; i < count; ++i)
            {
                if (m_tempGameObjectList[i].name == name)
                {
                    result = m_tempGameObjectList[i].transform;
                    break;
                }
            }
            m_tempGameObjectList.Clear();
            return result;
        }
    }
}
