using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonMoose.Core
{
    public class UIMediumResourceLoader<T> : UIMediumResourceLoader where T : UnityEngine.Object
    {
        private Action<string, UnityEngine.Object> m_actionOnLoadAsyncEnd;

        public override void OnRelease()
        {
            m_actionOnLoadAsyncEnd = null;
            base.OnRelease();
        }

        public override void OnLoad(bool isAsync)
        {
            if (isAsync)
            {
                if (m_actionOnLoadAsyncEnd == null)
                {
                    m_actionOnLoadAsyncEnd = OnLoadAsyncEnd;
                }
                ResourceManager.instance.LoadAsync<T>(m_path, m_actionOnLoadAsyncEnd);
            }
            else
            {
                T obj = ResourceManager.instance.LoadSync<T>(m_path);
                OnLoadAsyncEnd(m_path, obj);
            }
        }

        private void OnLoadAsyncEnd(string path, UnityEngine.Object obj)
        {
            NotifyLoadEnd();
        }
    }

    public abstract class UIMediumResourceLoader : ClassPoolObj
    {
        protected string m_path;
        protected Action<string> m_actionOnLoadEnd;

        public string path
        {
            get { return m_path; }
        }

        public void Set(string path)
        {
            m_path = path;
        }

        public override void OnRelease()
        {
            m_path = null;
            m_actionOnLoadEnd = null;
            base.OnRelease();
        }

        public void StartLoad(bool isAsync, Action<string> actionOnLoadEnd)
        {
            m_actionOnLoadEnd = actionOnLoadEnd;
            OnLoad(isAsync);
        }

        public abstract void OnLoad(bool isAsync);

        protected void NotifyLoadEnd()
        {
            if (m_actionOnLoadEnd != null)
            {
                m_actionOnLoadEnd(m_path);
            }
        }
    }
}
