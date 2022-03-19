using System;
using System.Collections;
using System.Collections.Generic;

namespace MonMoose.Core
{
    public class ProcessBase : ClassPoolObj
    {
        private EProcessState m_state;
        private Timer m_timer;

        public Action<ProcessBase> actionOnEnd;

        public virtual bool canStart { get { return true; } }
        public EProcessState state { get { return m_state; } }

        public void Init()
        {
            OnInit();
        }

        private void OnTimeUp()
        {
            End();
        }

        public void UnInit()
        {
            OnUnInit();
        }

        public void Start()
        {
            if (m_state != EProcessState.None && m_state != EProcessState.Ended)
            {
                DebugUtility.LogError("Cannot Start Process state is " + m_state);
                return;
            }
            m_state = EProcessState.Started;
            OnStart();
        }

        public void End()
        {
            if (m_state != EProcessState.Started)
            {
                DebugUtility.LogError("Cannot End Process state is " + m_state);
                return;
            }
            m_state = EProcessState.Ended;
            OnEnd();
            if (actionOnEnd != null)
            {
                Action<ProcessBase> temp = actionOnEnd;
                actionOnEnd = null;
                temp(this);
            }
        }

        public void Pause()
        {
            if (m_state != EProcessState.Started)
            {
                DebugUtility.LogError("Cannot Pause Process state is " + m_state);
                return;
            }
            m_state = EProcessState.Paused;
            OnPause();
        }

        public void Resume()
        {
            if (m_state != EProcessState.Paused)
            {
                DebugUtility.LogError("Cannot Resume Process state is " + m_state);
                return;
            }
            m_state = EProcessState.Started;
            OnResume();
        }

        public void Skip()
        {
            if (m_state != EProcessState.None)
            {
                DebugUtility.LogError("Cannot Skip Process state is " + m_state);
                return;
            }
            m_state = EProcessState.Ended;
            OnSkip();
        }

        public void DelayEnd(float time, bool exceptZero = true)
        {
            if (time > float.Epsilon || !exceptZero)
            {
                if (m_timer == null)
                {
                    m_timer = ClassPoolManager.instance.Fetch<Timer>(this);
                }
                m_timer.Start(time, OnTimeUp);
            }
            else
            {
                End();
            }
        }

        public void StopDelayEnd(bool callEnd)
        {
            if (m_timer == null)
            {
                return;
            }
            if (callEnd)
            {
                m_timer.Finish();
            }
            else
            {
                m_timer.Stop();
            }
        }

        public void Tick(float deltaTime)
        {
            if (m_timer != null)
            {
                m_timer.Tick(deltaTime);
            }
            OnTick(deltaTime);
        }

        public override void OnRelease()
        {
            actionOnEnd = null;
            m_state = EProcessState.None;
            if (m_timer != null)
            {
                m_timer.Release();
                m_timer = null;
            }
        }

        protected virtual void OnInit() { }
        protected virtual void OnUnInit() { }
        protected virtual void OnStart() { }
        protected virtual void OnEnd() { }
        protected virtual void OnPause() { }
        protected virtual void OnResume() { }
        protected virtual void OnSkip() { }
        protected virtual void OnTick(float deltaTime) { }
    }
}
