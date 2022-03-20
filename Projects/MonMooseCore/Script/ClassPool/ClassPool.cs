using System;
using System.Collections.Generic;

namespace MonMoose.Core
{
    public class ClassPool
    {
        public delegate void DelegateObject(object obj);
        public event DelegateObject actionOnFetch;
        public event DelegateObject actionOnRelease;

        private List<PoolObjHolder> m_holderList = new List<PoolObjHolder>();
        private Type m_classType;
        private int m_initCapacity = 4;

        public virtual Type classType
        {
            get { return m_classType; }
        }

        public void Init(Type type, int initCapacity = 0)
        {
            m_classType = type;
            if (initCapacity > 0)
            {
                m_initCapacity = initCapacity;
            }
            capacity = m_initCapacity;
        }

        public int capacity
        {
            get
            {
                lock (this)
                {
                    return m_holderList.Count;
                }
            }
            set
            {
                lock (this)
                {
                    if (value > m_holderList.Count)
                    {
                        m_holderList.Capacity = value;
                        for (int i = 0; i < value; ++i)
                        {
                            m_holderList.Add(CreateNew());
                        }
                    }
                }
            }
        }

        public object Fetch(object causer)
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
                    PoolObjHolder holder = CreateNew();
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
                SetCauser(poolObj, causer);
                poolObj.OnFetch();
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
                SetCauser(poolObj, null);
                poolObj.OnRelease();
            }
            OnRelease(obj);
            NotifyOnRelease(obj);
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
            capacity = m_initCapacity;
        }

        protected virtual void OnFetch(object obj) { }
        protected virtual void OnRelease(object obj) { }

        private PoolObjHolder CreateNew()
        {
            object obj = Activator.CreateInstance(classType);
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

        private void SetCauser(IClassPoolObj poolObj, object causer)
        {
#if !RELEASE
            poolObj.causer = causer;
#endif
        }

        private void NotifyOnFetch(object obj)
        {
            if (actionOnFetch != null)
            {
                actionOnFetch(obj);
            }
        }

        private void NotifyOnRelease(object obj)
        {
            if (actionOnRelease != null)
            {
                actionOnRelease(obj);
            }
        }

        private struct PoolObjHolder
        {
            public object obj;
            public bool isUsing;
        }
    }
}