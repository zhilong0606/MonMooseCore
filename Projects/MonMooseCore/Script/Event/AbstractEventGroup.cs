using System;
using System.Collections.Generic;

namespace MonMoose.Core
{
    public abstract class AbstractEventGroup
    {
        protected List<Delegate> m_delegateList = new List<Delegate>();

        public Delegate this[int index]
        {
            get { return m_delegateList[index]; }
        }

        public int count
        {
            get { return m_delegateList.Count; }
        }

        protected bool Contains(Delegate d)
        {
            return m_delegateList.Contains(d);
        }

        protected void Register(Delegate d)
        {
            if (d == null)
            {
                return;
            }
            if (m_delegateList.Count > 0 && d.GetType() != m_delegateList[0].GetType())
            {
                DebugUtility.LogError("Error: More Than One ParamGroups!!!");
                return;
            }
            if (!m_delegateList.Contains(d))
            {
                m_delegateList.Add(d);
            }
        }

        protected void UnRegister(Delegate d)
        {
            if (d == null)
            {
                return;
            }
            if (m_delegateList != null)
            {
                m_delegateList.Remove(d);
            }
        }

        protected void LogErrorNotMatchBroadcast()
        {
            DebugUtility.LogError("Error: Find Event Not Match ParamGroups!!!");
        }

        protected void HandleException(Exception e)
        {
            DebugUtility.LogError(e.ToString());
        }
    }
}
