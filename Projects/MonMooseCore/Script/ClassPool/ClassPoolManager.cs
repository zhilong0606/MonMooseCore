using System;
using System.Collections;
using System.Collections.Generic;

namespace MonMoose.Core
{
    public class ClassPoolManager : Singleton<ClassPoolManager>
    {
        private Dictionary<Type, ClassPool> m_poolMap = new Dictionary<Type, ClassPool>();

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
            int count = objList.Count;
            for (int i = 0; i < count; ++i)
            {
                Release(objList[i]);
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
                if (!m_poolMap.TryGetValue(type, out pool))
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
                    m_poolMap.Add(type, pool);
                }
            }
            return pool;
        }
    }
}
