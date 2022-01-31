using System;
using System.Collections.Generic;

namespace MonMooseCore
{
    public class ConditionGroup : ConditionBase
    {
        public Action<bool> actionOnValueChanged;

        private ConditionBool[] m_values;
        private ConditionBase m_rootCondition;
        private List<ConditionBase> m_cachedList = new List<ConditionBase>();
        private List<ConditionBase> m_checkDupList = new List<ConditionBase>();

        public ConditionBool this[int idx]
        {
            get
            {
                if (idx >= 0 && idx < m_values.Length)
                {
                    return m_values[idx];
                }
                return null;
            }
        }

        public void Resize(int count)
        {
            Clear();
            m_values = new ConditionBool[count];
            for (int i = 0; i < m_values.Length; ++i)
            {
                m_values[i] = NewValue();
            }
        }

        public void ClearCache()
        {
            for (int i = 0; i < m_cachedList.Count; ++i)
            {
                m_cachedList[i].Release();
            }
            m_cachedList.Clear();
            m_rootCondition = null;
        }

        public void SetRoot(ConditionBase root)
        {
            if (root == m_rootCondition)
            {
                return;
            }
            ClearCache();
            if (root != null)
            {
                m_rootCondition = root;
                for (int i = 0; i < m_values.Length; ++i)
                {
                    ConditionBase c = m_values[i].parent;
                    m_checkDupList.Clear();
                    while (c != null)
                    {
                        if (m_checkDupList.Contains(c))
                        {
                            DebugUtility.LogError("Error : Condition is Duplicated!!!!");
                            break;
                        }
                        if (m_cachedList.Contains(c))
                        {
                            break;
                        }
                        m_cachedList.Add(c);
                        m_checkDupList.Add(c);
                        c = c.parent;
                    }
                }
            }
            m_checkDupList.Clear();
        }

        public void Clear()
        {
            if (m_values != null)
            {
                for (int i = 0; i < m_values.Length; ++i)
                {
                    m_values[i].Release();
                    m_values[i] = null;
                }
            }
            m_values = null;
            actionOnValueChanged = null;
            ClearCache();
        }

        public override void Reset()
        {
            Clear();
            base.Reset();
        }

        private void OnConditionValueChanged()
        {
            if (actionOnValueChanged != null)
            {
                actionOnValueChanged(Check());
            }
        }

        public override bool Check()
        {
            return m_rootCondition.Check();
        }

        private ConditionBool NewValue()
        {
            ConditionBool condition = ClassPoolManager.instance.Fetch<ConditionBool>(this);
            InitValue(condition);
            return condition;
        }

        private void InitValue(ConditionBool value)
        {
            value.actionOnValueChanged = OnConditionValueChanged;
        }
    }
}