using System;
using System.Collections.Generic;
using UnityEngine;

namespace MonMoose.Core
{
    public class ResourceManager : Singleton<ResourceManager>
    {
        private ResourceLoader m_loader;

        public bool isInitialized
        {
            get { return m_loader != null && m_loader.isInitialized; }
        }

        protected override void OnInit()
        {
            base.OnInit();
            TickManager.instance.RegisterGlobalTick(OnTick);
        }

        protected override void OnUnInit()
        {
            base.OnUnInit();
            TickManager.instance.UnRegisterGlobalTick(OnTick);
        }

        public void InitLoader(bool useAssetBundle)
        {
#if !UNITY_EDITOR
            useAssetBundle = true;
#endif
            if (useAssetBundle)
            {
                m_loader = new ResourceLoaderAssetBundle();
            }
            else
            {
                m_loader = new ResourceLoaderAssetDatabase();
            }
            m_loader.Init();
        }

        public T LoadSync<T>(string resourceFullPath) where T : UnityEngine.Object
        {
            if (m_loader == null)
            {
                InitLoader(false);
            }
            return m_loader.LoadSync<T>(resourceFullPath);
        }

        public void LoadAsync<T>(string path, Action<string, UnityEngine.Object> actionOnLoadEnd) where T : UnityEngine.Object
        {
            if (m_loader == null)
            {
                InitLoader(false);
            }
            m_loader.LoadAsync<T>(path, actionOnLoadEnd);
        }

        private void OnTick(TimeSlice timeSlice)
        {
            m_loader.Tick(timeSlice);
        }
    }
}
