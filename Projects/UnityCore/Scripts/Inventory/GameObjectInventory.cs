using UnityEngine;

namespace MonMoose.Core
{
    public class GameObjectInventory : Inventory<GameObject>
    {
        public T GetComponent<T>(int index) where T : Component
        {
            GameObject go = Get(index);
            if (go == null)
            {
                return null;
            }
            T component = go.GetComponent<T>();
            return component;
        }

        public T AddComponent<T>(int index) where T : Component
        {
            GameObject go = Get(index);
            if (go == null)
            {
                return null;
            }
            T component = go.AddComponent<T>();
            if (component == null)
            {
                Debug.LogError(typeof(T).ToString());
            }
            return component;
        }
    }
}
