using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonMoose.Core
{
    public class ResourceCacheClip : ClassPoolObj
    {
        public string path;

        private WeakReference<UnityEngine.Object> m_resourceObjRef;
        private Dictionary<string, WeakReference<UnityEngine.Object>> m_resourceObjRefMap = new Dictionary<string, WeakReference<UnityEngine.Object>>();

        public override void OnRelease()
        {
            path = null;
            m_resourceObjRef = null;
            m_resourceObjRefMap.Clear();
            base.OnRelease();
        }

        public void Set(UnityEngine.Object resourceObj)
        {
            m_resourceObjRef = new WeakReference<UnityEngine.Object>(resourceObj);
        }

        public void Set(UnityEngine.Object[] resourceObjs)
        {
            m_resourceObjRefMap.Clear();
            if (resourceObjs != null)
            {
                int count = resourceObjs.Length;
                for (int i = 0; i < count; ++i)
                {
                    m_resourceObjRefMap.Add(resourceObjs[i].name, new WeakReference<UnityEngine.Object>(resourceObjs[i]));
                }
            }
        }

        public bool TryGet(out UnityEngine.Object resourceObj)
        {
            if (m_resourceObjRef.TryGetTarget(out resourceObj))
            {
                return true;
            }
            m_resourceObjRef = null;
            return false;
        }

        public bool TryGet(string name, out UnityEngine.Object resourceObj)
        {
            WeakReference<UnityEngine.Object> weakRef;
            if (m_resourceObjRefMap.TryGetValue(name, out weakRef))
            {
                if (weakRef.TryGetTarget(out resourceObj))
                {
                    return true;
                }
            }
            m_resourceObjRefMap.Remove(name);
            resourceObj = null;
            return false;
        }
    }
}
