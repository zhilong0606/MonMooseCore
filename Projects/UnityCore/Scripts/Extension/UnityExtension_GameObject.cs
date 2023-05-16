using UnityEngine;

namespace MonMoose.Core
{
    public static partial class UnityExtension
    {
        public static void SetActiveSafely(this GameObject go, bool isActive)
        {
            if (go != null && go.activeSelf != isActive)
            {
                go.SetActive(isActive);
            }
        }

        public static GameObject FindChild(this GameObject go, string path)
        {
            if (go != null)
            {
                Transform trans = go.transform.Find(path);
                if (trans != null)
                {
                    return trans.gameObject;
                }
                Debug.LogError("Error: Cannot Find Child by Path!");
            }
            else
            {
                Debug.LogError("Error: Left Value is Null!");
            }
            return null;
        }

        public static bool IsParentOf(this GameObject go, GameObject child)
        {
            if (go == null)
            {
                Debug.LogError("Error: Left Value is Null!");
                return false;
            }
            if (child == null)
            {
                Debug.LogError("Error: Right Value is Null!");
                return false;
            }
            return child.transform.parent == go.transform;
        }

        public static void SetParent(this GameObject go, GameObject parent)
        {
            if (go == null)
            {
                Debug.LogError("Error: Left Value is Null!");
                return;
            }
            if (parent == null)
            {
                Debug.LogError("Error: Right Value is Null!");
                return;
            }
            if (go.transform.parent != parent.transform)
            {
                go.transform.SetParent(parent.transform);
            }
        }

        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            if (go == null)
            {
                return null;
            }
            T component = go.GetComponent<T>();
            if (component == null)
            {
                component = go.AddComponent<T>();
            }
            return component;
        }

        public static T GetComponentSafely<T>(this GameObject go) where T : Component
        {
            if (go == null)
            {
                return null;
            }
            return go.GetComponent<T>();
        }
    }
}
