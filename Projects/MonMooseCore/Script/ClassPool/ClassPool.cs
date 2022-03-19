using System;
using System.Collections.Generic;

namespace MonMoose.Core
{
    public class ClassPool
    {
        public delegate void DelegateObject(object obj);

        protected List<object> m_objList = new List<object>();
        protected Type m_classType;

        public event DelegateObject actionOnFetch;
        public event DelegateObject actionOnRelease;
        private readonly List<object> m_spareList = new List<object>();

        public virtual Type classType
        {
            get { return m_classType; }
        }

        public int capacity
        {
            set
            {
                lock (this)
                {
                    int totalCount = m_objList.Count + m_spareList.Count;
                    if (value > totalCount)
                    {
                        m_spareList.Capacity = value;
                        m_objList.Capacity = value;
                        int newCount = value - m_objList.Count;
                        for (int i = 0; i < newCount; ++i)
                        {
                            m_spareList.Add(CreateObj());
                        }
                    }
                }
            }
        }

        public void Init(Type type)
        {
            m_classType = type;
        }

        public object Fetch(object causer)
        {
            object obj;
            lock (this)
            {
                if (m_spareList.Count > 0)
                {
                    obj = m_spareList[m_spareList.Count - 1];
                    m_spareList.RemoveAt(m_spareList.Count - 1);
                }
                else
                {
                    obj = CreateObj();
                }
                m_objList.Add(obj);
            }
            IClassPoolObj poolObj = obj as IClassPoolObj;
            if (poolObj != null)
            {
#if !RELEASE
                poolObj.causer = causer;
#endif
                poolObj.OnFetch();
            }
            if (actionOnFetch != null)
            {
                actionOnFetch(obj);
            }
            return obj;
        }

        public void Release(object obj)
        {
            lock (this)
            {
                if (!m_objList.Contains(obj))
                {
                    throw new Exception(string.Format("Error: Trying to destroy object that is not create from this pool. {0} => {1}", obj.GetType().Name, classType.Name));
                }
                if (m_spareList.Contains(obj))
                {
                    throw new Exception("Error: Trying to destroy object that is already released to pool.");
                }
                m_objList.Remove(obj);
                m_spareList.Add(obj);
            }
            IClassPoolObj poolObj = obj as IClassPoolObj;
            if (poolObj != null)
            {
#if !RELEASE
                poolObj.causer = null;
#endif
                poolObj.creator = null;
                poolObj.OnRelease();
            }
            if (actionOnRelease != null)
            {
                actionOnRelease(obj);
            }
        }

        private object CreateObj()
        {
            object obj = Activator.CreateInstance(classType);
            IClassPoolObj poolObj = obj as IClassPoolObj;
            if (poolObj != null)
            {
                poolObj.creator = this;
            }
            return obj;
        }
    }
}