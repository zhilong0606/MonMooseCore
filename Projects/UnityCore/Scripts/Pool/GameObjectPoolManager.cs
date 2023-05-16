using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonMoose.Core
{
    public class GameObjectPoolManager : MonoSingleton<GameObjectPoolManager>
    {
        [SerializeField]
        private GameObjectPool m_pPool;

        private Dictionary<string, GameObjectPool> m_poolMap = new Dictionary<string, GameObjectPool>();

        public GameObjectPool CreatePool(string prefabPath, GameObjectPool.ActionObjHolder actionOnInit = null, int capacity = 0)
        {
            GameObject prefab = AssetUtility.GetPrefab(prefabPath);
            return CreatePool(prefab, prefabPath, actionOnInit, capacity);
        }

        public GameObjectPool CreatePool(GameObject go, string name, GameObjectPool.ActionObjHolder actionOnInit = null, int capacity = 0)
        {
            GameObjectPool pool;
            if (m_poolMap.TryGetValue(name, out pool))
            {
                Debug.LogError(string.Format("[GameObjectPoolManager] Pool({0}) is Already Exist", name));
            }
            pool = m_pPool.FetchComponent<GameObjectPool>();
            pool.name = name;
            pool.Init(go, actionOnInit);
            pool.capacity = capacity;
            m_poolMap.Add(name, pool);
            return pool;
        }

        public GameObject Fetch(string prefabName)
        {
            GameObjectPool pool = GetPool(prefabName);
            if (pool == null)
            {
                pool = CreatePool(prefabName);
            }
            return pool.Fetch().obj;
        }

        public GameObject Fetch(string prefabName, GameObject parentObj = null)
        {
            GameObjectPool pool = GetPool(prefabName);
            if (pool == null)
            {
                pool = CreatePool(prefabName);
            }
            return pool.Fetch(parentObj).obj;
        }

        public bool Release(GameObject go)
        {
            foreach (var kv in m_poolMap)
            {
                if (kv.Value.Release(go))
                {
                    return true;
                }
            }
            return false;
        }


        private GameObjectPool GetPool(string name)
        {
            GameObjectPool pool;
            if (m_poolMap.TryGetValue(name, out pool))
            {
                return pool;
            }
            return null;
        }
    }
}
