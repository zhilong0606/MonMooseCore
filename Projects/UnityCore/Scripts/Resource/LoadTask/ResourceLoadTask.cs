using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonMoose.Core
{
    public abstract class ResourceLoadTask : ClassPoolObj
    {
        public string fullPath;//loadPath or loadPath@subName
        public string loadPath;
        public string subName;
        public UnityEngine.Object resourceObj;

        public Action<ResourceLoadTask> actionOnLoadEnd;
        private List<Action<string, UnityEngine.Object>> m_actionOnLoadObjectEndList = new List<Action<string, UnityEngine.Object>>();
        private const float m_lag =  0f;//0.5f;
        private Timer m_lagTimer;

        private EState m_state;

        public bool isLoadEnd
        {
            get { return m_state == EState.End; }
        }

        protected virtual bool needLag
        {
            get { return true; }
        }

        public override void OnRelease()
        {
            m_state = EState.None;
            fullPath = null;
            loadPath = null;
            subName = null;
            resourceObj = null;
            actionOnLoadEnd = null;
            if (m_lagTimer != null)
            {
                m_lagTimer.Release();
                m_lagTimer = null;
            }
            m_actionOnLoadObjectEndList.Clear();
        }

        public void StartLoad()
        {
            m_state = EState.Start;
        }

        public void NotifyLoadObjectEnd()
        {
            for (int i = 0; i < m_actionOnLoadObjectEndList.Count; ++i)
            {
                m_actionOnLoadObjectEndList[i].InvokeSafely(fullPath, resourceObj);
            }
        }

        public void AddActionOnLoadObjectEnd(Action<string, UnityEngine.Object> action)
        {
            if (!m_actionOnLoadObjectEndList.Contains(action))
            {
                m_actionOnLoadObjectEndList.Add(action);
            }
        }

        public void Tick(TimeSlice timeSlice)
        {
            TickState();
            if (m_lagTimer != null)
            {
                m_lagTimer.Tick(timeSlice);
            }
        }

        private void TickState()
        {
            if (m_state == EState.Start)
            {
                m_state = EState.Loading;
                OnLoadStart();
                TickState();
            }
            else if (m_state == EState.Loading)
            {
                OnTickLoading();
                if (CheckLoadEnd())
                {
                    m_state = EState.Lag;
                    TickState();
                }
            }
            else if( m_state == EState.Lag)
            {
                if (m_lag > 1e-4 && needLag)
                {
                    if (m_lagTimer == null)
                    {
                        m_lagTimer = ClassPoolManager.instance.Fetch<Timer>();
                    }
                    m_lagTimer.Start(m_lag);
                    m_state = EState.Lagging;
                    TickState();
                }
                else
                {
                    m_state = EState.Ending;
                    TickState();
                }
            }
            else if (m_state == EState.Lagging)
            {
                if (m_lagTimer.isFinished)
                {
                    m_state = EState.Ending;
                    TickState();
                }
            }
            else if (m_state == EState.Ending)
            {
                m_state = EState.End;
                NotifyLoadEnd();
                NotifyLoadObjectEnd();
            }
        }

        protected void NotifyLoadEnd()
        {
            actionOnLoadEnd.InvokeSafely(this);
        }

        protected virtual void OnLoadStart() { }
        protected virtual void OnTickLoading() { }
        protected abstract bool CheckLoadEnd();

        private enum EState
        {
            None,
            Start,
            Loading,
            Ending,
            Lag,
            Lagging,
            End,
        }
    }
}
