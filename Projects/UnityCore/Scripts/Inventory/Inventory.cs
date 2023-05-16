using UnityEngine;

namespace MonMoose.Core
{
    public class Inventory<T> : MonoBehaviour
    {
        [SerializeField] private T[] m_items = new T[0];

        public T this[int index]
        {
            get { return m_items[index]; }
        }

        public int Count
        {
            get { return m_items.Length; }
        }

        public T Get(int index)
        {
            if (index >= 0 && index < m_items.Length)
            {
                return m_items[index];
            }
            return default(T);
        }
    }
}
