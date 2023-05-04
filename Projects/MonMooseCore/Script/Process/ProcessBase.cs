using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace MonMoose.Core
{
    public class ProcessBase : ClassPoolObj
    {
        private ProcessStateId m_state;
        private Timer m_timer;
#if !RELEASE
        private static int m_idCursor = 1;
        private int m_id;
        private Dictionary<int, StackTrace> m_stackTraceMap = new Dictionary<int, StackTrace>();
        private List<Tuple<string, StackTrace>> m_stackTraceLogList = new List<Tuple<string, StackTrace>>();
        private ProcessBase m_parentProcess;

        public ProcessBase()
        {
            m_id = m_idCursor++;
        }
#endif

        public Action<ProcessBase> actionOnEnd;

        public ProcessStateId state { get { return m_state; } }
        public virtual bool canStart { get { return true; } }

        public bool isStarted
        {
            get { return m_state == ProcessStateId.Started || m_state == ProcessStateId.Paused; }
        }

        public bool canUnInit
        {
            get { return m_state != ProcessStateId.None && m_state != ProcessStateId.UnInited; }
        }

        public override void OnRelease()
        {
            //if (canUnInit)
            //{
            //    UnInit();
            //}
            actionOnEnd = null;
#if !RELEASE
            m_parentProcess = null;
            m_stackTraceLogList.Clear();
            m_stackTraceMap.Clear();
            m_id = default;
#endif
            m_state = ProcessStateId.None;
            if (m_timer != null)
            {
                m_timer.Release();
                m_timer = null;
            }
        }

        public void Init()
        {
            AddStackTraceLog("Init");
            if (m_state != ProcessStateId.None && m_state != ProcessStateId.UnInited)
            {
                DebugUtility.LogError("Cannot Init Process state is " + m_state);
#if !RELEASE
                DebugUtility.LogError("[ProcessBase] " + m_stackTraceMap[(int)ProcessStateId.Inited]);
                DebugUtility.LogError("[ProcessBase] " + new StackTrace());
#endif
                return;
            }
#if !RELEASE
            m_stackTraceMap[(int)ProcessStateId.Inited] = new StackTrace();
#endif
            m_state = ProcessStateId.Inited;
            OnInit();
        }

        public void UnInit()
        {
            AddStackTraceLog("UnInit");
            if (!canUnInit)
            {
                DebugUtility.LogError("Cannot UnInit Process state is " + m_state);
#if !RELEASE
                DebugUtility.LogError("[ProcessBase] " + m_stackTraceMap[(int)ProcessStateId.UnInited]);
                DebugUtility.LogError("[ProcessBase] " + new StackTrace());
#endif
                return;
            }
#if !RELEASE
            m_stackTraceMap[(int)ProcessStateId.UnInited] = new StackTrace();
#endif
            m_state = ProcessStateId.UnInited;
            OnUnInit();
        }

        public void Start()
        {
            AddStackTraceLog("Start");
            if (m_state != ProcessStateId.Inited)
            {
                DebugUtility.LogError("Cannot Start Process state is " + m_state);
#if !RELEASE
                DebugUtility.LogError("[ProcessBase] " + m_stackTraceMap[(int)ProcessStateId.Started]);
                DebugUtility.LogError("[ProcessBase] " + new StackTrace());
#endif
                return;
            }
#if !RELEASE
            m_stackTraceMap[(int)ProcessStateId.Started] = new StackTrace();
#endif
            m_state = ProcessStateId.Started;
            OnStart();
        }

        public bool InitAndStartAndCheckRunning()
        {
            Init();
            if (canStart)
            {
                Start();
                return isStarted;
            }
            Skip();
            return false;
        }

        public void End()
        {
            AddStackTraceLog("End");
            if (m_state != ProcessStateId.Inited && m_state != ProcessStateId.Started)
            {
                DebugUtility.LogError("Cannot End Process state is " + m_state);
#if !RELEASE
                DebugUtility.LogError("[ProcessBase] " + m_stackTraceMap[(int)ProcessStateId.Ended]);
                DebugUtility.LogError("[ProcessBase] " + new StackTrace());
#endif
                return;
            }
#if !RELEASE
            m_stackTraceMap[(int)ProcessStateId.Ended] = new StackTrace();
#endif
            m_state = ProcessStateId.Ended;
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
            AddStackTraceLog("Pause");
            if (m_state != ProcessStateId.Started)
            {
                DebugUtility.LogError("Cannot Pause Process state is " + m_state);
                return;
            }
            m_state = ProcessStateId.Paused;
            OnPause();
        }

        public void Resume()
        {
            AddStackTraceLog("Resume");
            if (m_state != ProcessStateId.Paused)
            {
                DebugUtility.LogError("Cannot Resume Process state is " + m_state);
                return;
            }
            m_state = ProcessStateId.Started;
            OnResume();
        }

        public void Skip()
        {
            AddStackTraceLog("Skip");
            if (m_state != ProcessStateId.Inited)
            {
                DebugUtility.LogError("Cannot Skip Process state is " + m_state);
                return;
            }
            m_state = ProcessStateId.Ended;
            OnSkip();
        }

        public void DelayEnd(float time, bool exceptZero = true)
        {
            if (time > float.Epsilon || !exceptZero)
            {
                if (m_timer == null)
                {
                    m_timer = ClassPoolManager.instance.Fetch<Timer>();
                    m_timer.checkPointId = checkPointId;
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

        public void Tick(TimeSlice timeSlice)
        {
            if (m_state != ProcessStateId.Started)
            {
                return;
            }
            if (m_timer != null)
            {
                m_timer.Tick(timeSlice);
            }
            OnTick(timeSlice);
        }

        public void SetParent(ProcessBase parent)
        {
#if !RELEASE
            m_parentProcess = parent;
#endif
        }

        private void AddStackTraceLog(string tag)
        {
#if !RELEASE
            m_stackTraceLogList.Add(new Tuple<string, StackTrace>(tag, new StackTrace()));
#endif
        }

        private void OnTimeUp()
        {
            End();
        }

        protected virtual void OnInit() { }
        protected virtual void OnUnInit() { }
        protected virtual void OnStart() { }
        protected virtual void OnEnd() { }
        protected virtual void OnPause() { }
        protected virtual void OnResume() { }
        protected virtual void OnSkip() { }
        protected virtual void OnTick(TimeSlice timeSlice) { }
    }
}
