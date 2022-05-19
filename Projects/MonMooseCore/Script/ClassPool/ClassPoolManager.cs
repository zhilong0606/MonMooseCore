using System;
using System.Collections;
using System.Collections.Generic;

namespace MonMoose.Core
{
    public class ClassPoolManager : Singleton<ClassPoolManager>
    {
        private Dictionary<Type, ClassPool> m_poolMap = new Dictionary<Type, ClassPool>();

        //尽量填causer，static类的话填Type，其他填this
        public T Fetch<T>(object causer) where T : class
        {
            ClassPool pool = GetPool(typeof(T));
            return pool.Fetch(causer) as T;
        }

        //尽量填causer，static类的话填Type，其他填this
        public object Fetch(Type type, object causer)
        {
            ClassPool pool = GetPool(type);
            return pool.Fetch(causer);
        }

        public List<T> FetchList<T>(object causer)
        {
            ClassPool pool = GetPool(typeof(List<T>));
            return pool.Fetch(causer) as List<T>;
        }

        public void Release(object obj)
        {
            if (obj == null)
            {
                return;
            }
            ClassPool pool = GetPool(obj.GetType());
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

        public void SetCapacity(Type type, int capacity)
        {
            ClassPool pool = GetPool(type);
            pool.capacity = capacity;
        }

        public void RegisterOnFetch(Type type, ClassPool.DelegateObject action)
        {
            ClassPool pool = GetPool(type);
            pool.actionOnFetch += action;
        }

        public void UnRegisterOnFetch(Type type, ClassPool.DelegateObject action)
        {
            ClassPool pool = GetPool(type);
            pool.actionOnFetch -= action;
        }

        public void RegisterOnRelease(Type type, ClassPool.DelegateObject action)
        {
            ClassPool pool = GetPool(type);
            pool.actionOnRelease += action;
        }

        public void UnRegisterOnRelease(Type type, ClassPool.DelegateObject action)
        {
            ClassPool pool = GetPool(type);
            pool.actionOnRelease -= action;
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
