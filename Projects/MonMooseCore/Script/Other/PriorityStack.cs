using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonMoose.Core
{
    public class PriorityStack<T>
    {
        public delegate void DelegateOnTopChanged(PriorityStack<T> priorityStack);

        private static readonly List<int> m_tempIntList = new List<int>();

        private Dictionary<int, T> m_cachedInfoMap = new Dictionary<int, T>();
        private int m_topPriority;
        private int m_defaultPriority;
        private bool m_isInitialized;
        public string name;

        private DelegateOnTopChanged m_actionOnTopChanged;

        public int topPriority
        {
            get { return m_topPriority; }
        }


        public T topValue
        {
            get { return m_cachedInfoMap[m_topPriority]; }
        }

        public T defaultValue
        {
            get { return m_cachedInfoMap[m_defaultPriority]; }
        }

        public bool isEmpty
        {
            get { return m_cachedInfoMap.Count == 0; }
        }

        public void Init(int defaultPriority, T defaultValue, DelegateOnTopChanged actionOnTopChanged)
        {
            if (m_isInitialized)
            {
                return;
            }
            m_isInitialized = true;
            m_defaultPriority = defaultPriority;
            m_actionOnTopChanged = actionOnTopChanged;
            AddItem(defaultPriority, defaultValue);
        }

        public void Uninit()
        {
            m_isInitialized = false;
            m_cachedInfoMap.Clear();
            m_topPriority = 0;
            m_defaultPriority = 0;
        }

        public bool IsTop(int priority)
        {
            return priority >= m_topPriority;
        }

        public void AddItem(int priority, T item)
        {
            if (!m_isInitialized)
            {
                return;
            }
            if (priority < m_defaultPriority)
            {
                DebugUtility.LogError("[PriorityStack] Cannot Add Priority Less Than Default");
                return;
            }
            m_cachedInfoMap[priority] = item;
            if (IsTop(priority))
            {
                ChangeTop(priority);
            }
        }

        public void Remove(int priority)
        {
            if (!m_isInitialized)
            {
                return;
            }
            if (priority == m_defaultPriority)
            {
                DebugUtility.LogError("[PriorityStack] Cannot Remove Default Priority");
                return;
            }
            if (!m_cachedInfoMap.ContainsKey(priority))
            {
                return;
            }
            T oldTop = topValue;
            m_cachedInfoMap.Remove(priority);
            if (!IsTop(priority))
            {
                return;
            }
            RefreshTop(oldTop);
        }

        public void RemoveAll(params int[] exceptPriorities)
        {
            if (!m_isInitialized)
            {
                return;
            }
            bool needRefreshTop = false;
            foreach (var kv in m_cachedInfoMap)
            {
                bool needRemove = kv.Key != m_defaultPriority;
                if (needRemove)
                {
                    for (int i = 0; i < exceptPriorities.Length; ++i)
                    {
                        int exceptPriority = exceptPriorities[i];
                        if (kv.Key == exceptPriority)
                        {
                            needRemove = false;
                            break;
                        }
                    }
                }
                if (needRemove)
                {
                    if (IsTop(kv.Key))
                    {
                        needRefreshTop = true;
                    }
                    m_tempIntList.Add(kv.Key);
                }
            }
            T oldTop = topValue;
            for (int i = 0; i < m_tempIntList.Count; ++i)
            {
                m_cachedInfoMap.Remove(m_tempIntList[i]);
            }
            m_tempIntList.Clear();
            if (needRefreshTop)
            {
                RefreshTop(oldTop);
            }
        }

        private void RefreshTop(T oldTop)
        {
            int newPriority = int.MinValue;
            foreach (var kv in m_cachedInfoMap)
            {
                if (kv.Key > newPriority)
                {
                    newPriority = kv.Key;
                }
            }
            ChangeTop(newPriority);
        }

        private void ChangeTop(int property)
        {
            m_topPriority = property;
            if (m_actionOnTopChanged != null)
            {
                m_actionOnTopChanged(this);
            }
        }
    }
}
