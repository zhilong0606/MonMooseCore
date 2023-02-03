using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MonMoose.Core
{
    public class ClassPool
    {
        private List<Action<object>> m_actionOnFetchList = new List<Action<object>>();
        private List<Action<object>> m_actionOnReleaseList = new List<Action<object>>();

        private List<PoolObjHolder> m_holderList = new List<PoolObjHolder>();
        protected Type m_classType;
        private static readonly Regex m_logRegex = new Regex(@"at\s+(\w*\.)*(\w+\.\w+)");

        public virtual Type classType
        {
            get { return m_classType; }
        }

        public virtual string poolName
        {
            get { return m_classType.Name; }
        }

        public int usingCount
        {
            get
            {
                int count = 0;
                for (int i = 0; i < m_holderList.Count; ++i)
                {
                    if (m_holderList[i].isUsing)
                    {
                        count++;
                    }
                }
                return count;
            }
        }

        public void Init(Type type)
        {
            m_classType = type;
        }

        public int GetCapacity()
        {
            lock (this)
            {
                return m_holderList.Count;
            }
        }

        public void SetCapacity(int capacity, Func<Type, object> funcOnCreate = null)
        {
            lock (this)
            {
                if (capacity > m_holderList.Count)
                {
                    m_holderList.Capacity = capacity;
                    for (int i = m_holderList.Count; i < capacity; ++i)
                    {
                        m_holderList.Add(CreateNew(funcOnCreate));
                    }
                }
            }
        }

        public object Fetch(Func<Type, object> funcOnCreate = null)
        {
            object obj;
            lock (this)
            {
                int holderIndex = -1;
                for (int i = 0; i < m_holderList.Count; ++i)
                {
                    if (!m_holderList[i].isUsing)
                    {
                        holderIndex = i;
                        break;
                    }
                }
                if (holderIndex < 0)
                {
                    PoolObjHolder holder = CreateNew(funcOnCreate);
                    holder.isUsing = true;
                    m_holderList.Add(holder);
                    obj = holder.obj;
                }
                else
                {
                    PoolObjHolder holder = m_holderList[holderIndex];
                    holder.isUsing = true;
                    m_holderList[holderIndex] = holder;
                    obj = holder.obj;

                }
            }
            IClassPoolObj poolObj = obj as IClassPoolObj;
            if (poolObj != null)
            {
                poolObj.OnFetch();
                RecordCreateStackTraceAndLog(poolObj);
            }
            OnFetch(obj);
            NotifyOnFetch(obj);
            return obj;
        }

        public void Release(object obj)
        {
            lock (this)
            {
                int holderIndex = -1;
                for (int i = 0; i < m_holderList.Count; ++i)
                {
                    if (m_holderList[i].obj == obj)
                    {
                        holderIndex = i;
                        break;
                    }
                }
                if (holderIndex < 0)
                {
                    throw new Exception(string.Format("Error: Trying to destroy object that is not create from this pool. {0} => {1}", obj.GetType().Name, classType.Name));
                }
                PoolObjHolder holder = m_holderList[holderIndex];
                holder.isUsing = false;
                m_holderList[holderIndex] = holder;

            }
            IClassPoolObj poolObj = obj as IClassPoolObj;
            if (poolObj != null)
            {
                poolObj.OnRelease();
                poolObj.stackTrace = null;
                poolObj.createLog = null;
            }
            OnRelease(obj);
            NotifyOnRelease(obj);
        }

        public void RegisterActionOnFetch(Action<object> actionOnFetch)
        {
            if (actionOnFetch.Target != null)
            {
                DebugUtility.LogError("[ClassPool] Cannot Register ActionOnFetch is not Static");
                return;
            }
            if (m_actionOnFetchList.Contains(actionOnFetch))
            {
                return;
            }
            m_actionOnFetchList.Add(actionOnFetch);
        }

        public void UnRegisterActionOnFetch(Action<object> actionOnFetch)
        {
            if (!m_actionOnFetchList.Contains(actionOnFetch))
            {
                return;
            }
            m_actionOnFetchList.Remove(actionOnFetch);
        }

        public void RegisterActionOnRelease(Action<object> actionOnRelease)
        {
            if (actionOnRelease.Target != null)
            {
                DebugUtility.LogError("[ClassPool] Cannot Register ActionOnRelease is not Static");
                return;
            }
            if (m_actionOnReleaseList.Contains(actionOnRelease))
            {
                return;
            }
            m_actionOnReleaseList.Add(actionOnRelease);
        }

        public void UnRegisterActionOnRelease(Action<object> actionOnRelease)
        {
            if (!m_actionOnReleaseList.Contains(actionOnRelease))
            {
                return;
            }
            m_actionOnReleaseList.Remove(actionOnRelease);
        }

        public void Clear()
        {
            lock (this)
            {
                for (int i = 0; i < m_holderList.Count; ++i)
                {
                    IClassPoolObj poolObj = m_holderList[i].obj as IClassPoolObj;
                    if (poolObj != null)
                    {
                        poolObj.creator = null;
                    }
                }
                m_holderList.Clear();
            }
        }

        protected virtual void OnFetch(object obj) { }
        protected virtual void OnRelease(object obj) { }

        private PoolObjHolder CreateNew(Func<Type, object> funcOnCreate)
        {
            object obj = null;
            if (funcOnCreate != null)
            {
                obj = funcOnCreate(m_classType);
            }
            if (obj == null)
            {
                obj = Activator.CreateInstance(classType);
            }
            IClassPoolObj poolObj = obj as IClassPoolObj;
            if (poolObj != null)
            {
                poolObj.creator = this;
            }
            PoolObjHolder holder = new PoolObjHolder();
            holder.obj = obj;
            holder.isUsing = false;
            return holder;
        }

        private void NotifyOnFetch(object obj)
        {
            for (int i = 0; i < m_actionOnFetchList.Count; ++i)
            {
                try
                {
                    m_actionOnFetchList[i].Invoke(obj);
                }
                catch (Exception e)
                {
                    DebugUtility.LogError(e.ToString());
                }
            }
        }

        private void NotifyOnRelease(object obj)
        {
            for (int i = 0; i < m_actionOnReleaseList.Count; ++i)
            {
                try
                {
                    m_actionOnReleaseList[i].Invoke(obj);
                }
                catch (Exception e)
                {
                    DebugUtility.LogError(e.ToString());
                }
            }
        }

        private void RecordCreateStackTraceAndLog(IClassPoolObj poolObj)
        {
#if !RELEASE && DEBUG_CLASSPOOL
            System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace(true);
            MatchCollection matchCollection = m_logRegex.Matches(stackTrace.ToString());
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (Match match in matchCollection)
            {
                if (sb.Length != 0)
                {
                    sb.Append("-->");
                }
                sb.Append(match.Groups[match.Groups.Count - 1].Value);
            }
            poolObj.stackTrace = stackTrace;
            poolObj.createLog = sb.ToString();
#endif
        }

        private struct PoolObjHolder
        {
            public object obj;
            public bool isUsing;
            public int checkPointId;
        }
    }
}