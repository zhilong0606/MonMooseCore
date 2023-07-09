using System;
using System.Collections;
using System.Collections.Generic;

namespace MonMoose.Core
{
    public class ClassPoolManager : Singleton<ClassPoolManager>
    {
        private List<ClassPool> m_poolList = new List<ClassPool>();

        public T Fetch<T>(Func<Type, object> funcOnCreate = null) where T : class
        {
            return Fetch(typeof(T), funcOnCreate) as T;
        }

        public object Fetch(Type type, Func<Type, object> funcOnCreate = null)
        {
            ClassPool pool = GetPool(type);
            return pool.Fetch(funcOnCreate);
        }

        public void Release(object obj)
        {
            if (obj == null)
            {
                return;
            }
            IClassPoolObj poolObj = obj as IClassPoolObj;
            ClassPool pool = null;
            if (poolObj != null)
            {
                pool = poolObj.creator;
            }
            if (pool == null)
            {
                pool = GetPool(obj.GetType());
            }
            pool.Release(obj);
        }

        public void ReleaseAll(IList objList)
        {
            if (objList == null)
            {
                return;
            }
            int count = objList.Count;
            for (int i = 0; i < count; ++i)
            {
                Release(objList[i]);
            }
            objList.Clear();
        }

        public void ReleaseAll<T>(T[] objs) where T : IClassPoolObj
        {
            if (objs == null)
            {
                return;
            }
            int count = objs.Length;
            for (int i = 0; i < count; ++i)
            {
                Release(objs[i]);
                objs[i] = default(T);
            }
        }

        public void SetCapacity(Type type, int capacity, Func<Type, object> actionOnCreate = null)
        {
            ClassPool pool = GetPool(type);
            pool.SetCapacity(capacity, actionOnCreate);
        }

        public void RegisterActionOnFetch(Type type, Action<object> action)
        {
            ClassPool pool = GetPool(type);
            pool.RegisterActionOnFetch(action);
        }

        public void UnRegisterActionOnFetch(Type type, Action<object> action)
        {
            ClassPool pool = GetPool(type);
            pool.UnRegisterActionOnFetch(action);
        }

        public void RegisterActionOnRelease(Type type, Action<object> action)
        {
            ClassPool pool = GetPool(type);
            pool.RegisterActionOnRelease(action);
        }

        public void UnRegisterActionOnRelease(Type type, Action<object> action)
        {
            ClassPool pool = GetPool(type);
            pool.UnRegisterActionOnRelease(action);
        }

        public ClassPool GetPool(Type type)
        {
            ClassPool pool = null;
            lock (this)
            {
                for (int i = 0; i < m_poolList.Count; ++i)
                {
                    if (m_poolList[i].classType == type)
                    {
                        pool = m_poolList[i];
                    }
                }
                if (pool == null)
                {
                    if (typeof(IList).IsAssignableFrom(type))
                    {
                        pool = new ClassPoolList();
                    }
                    else
                    {
                        pool = new ClassPool();
                    }
                    pool.Init(type);
                    m_poolList.Add(pool);
                }
            }
            return pool;
        }

        public void LogOutPoolUsingCount()
        {
            DebugUtility.Log("[LogOutPool] LogOutPoolUsingCount Start");
            foreach (var kv in m_poolList)
            {
                int count = kv.usingCount;
                if (count > 0)
                {
                    string name = kv.poolName;
                    DebugUtility.Log(string.Format("[LogOutPool] LogOutPoolUsingCount: {0} : {1}", name, count));
                }
            }
            DebugUtility.Log("[LogOutPool] LogOutPoolUsingCount End");
        }
    }
}
