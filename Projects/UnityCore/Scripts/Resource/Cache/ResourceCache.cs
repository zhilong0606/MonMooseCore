using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonMoose.Core
{
    public class ResourceCache
    {
        private Dictionary<string, ResourceCacheClip> m_clipMap = new Dictionary<string, ResourceCacheClip>();

        public void Add(string path, UnityEngine.Object resourceObj)
        {
            ResourceCacheClip clip = GetOrCreateClip(path);
            clip.Set(resourceObj);
        }

        public void Add(string path, UnityEngine.Object[] resourceObjs)
        {
            ResourceCacheClip clip = GetOrCreateClip(path);
            clip.Set(resourceObjs);
        }

        public bool TryGet(string path, out UnityEngine.Object resourceObj)
        {
            ResourceCacheClip clip = GetClip(path);
            if (clip != null)
            {
                return clip.TryGet(path, out resourceObj);
            }
            resourceObj = null;
            return false;
        }

        public bool TryGet(string path, string name, out UnityEngine.Object resourceObj)
        {
            ResourceCacheClip clip = GetClip(path);
            if (clip != null)
            {
                return clip.TryGet(name, out resourceObj);
            }
            resourceObj = null;
            return false;
        }

        public void Clear()
        {
            foreach (var kv in m_clipMap)
            {
                kv.Value.Release();
            }
            m_clipMap.Clear();
        }

        private ResourceCacheClip GetOrCreateClip(string path)
        {
            ResourceCacheClip clip = GetClip(path);
            if (clip == null)
            {
                clip = ClassPoolManager.instance.Fetch<ResourceCacheClip>();
                clip.path = path;
                m_clipMap.Add(path, clip);
            }
            return clip;
        }

        private ResourceCacheClip GetClip(string path)
        {
            ResourceCacheClip clip;
            m_clipMap.TryGetValue(path, out clip);
            return clip;
        }
    }
}
