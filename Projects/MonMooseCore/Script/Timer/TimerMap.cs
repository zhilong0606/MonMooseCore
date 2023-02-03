using System;
using System.Collections.Generic;

namespace MonMoose.Core
{
    public class TimerMap
    {
        private Dictionary<int, Timer> m_timerMap = new Dictionary<int, Timer>();
        private List<Timer> m_timerList = new List<Timer>();
        private List<int> m_finishList = new List<int>();
        public Action<int> onTimeUp;
        public Action<int> onAddTimer;
        public Action<int> onRemoveTimer;
        public ClassPool pool;

        ~TimerMap()
        {
            Dictionary<int, Timer>.Enumerator enumerator = m_timerMap.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.Value != null)
                {
                    enumerator.Current.Value.Release();
                }
            }
            enumerator.Dispose();
            m_timerMap.Clear();
            onTimeUp = null;
        }

        public bool HasTimer(int id)
        {
            Timer timer;
            return m_timerMap.TryGetValue(id, out timer) && timer != null;
        }

        public Timer GetTimer(int id, bool force = false)
        {
            Timer timer = null;
            if (!m_timerMap.TryGetValue(id, out timer) || timer == null)
            {
                if (force)
                {
                    timer = AddTimer(id);
                }
            }
            return timer;
        }

        private Timer CreateTimer(int id)
        {
            Timer timer = null;
            if (pool != null)
            {
                timer = pool.Fetch() as Timer;
            }
            else
            {
                timer = new Timer();
            }
            if (timer != null)
            {
                timer.id = id;
            }
            return timer;
        }

        private Timer AddTimer(int id)
        {
            Timer timer = null;
            if (m_timerMap.ContainsKey(id))
            {
                if (m_timerMap[id] == null)
                {
                    timer = CreateTimer(id);
                    m_timerMap[id] = timer;
                }
            }
            else
            {
                timer = CreateTimer(id);
                m_timerMap.Add(id, timer);
            }
            if (timer != null)
            {
                onAddTimer(id);
            }
            return timer;
        }

        private void RemoveTimer(Timer timer)
        {
            RemoveTimer(timer.id);
        }

        private void RemoveTimer(int id)
        {
            Timer timer;
            if (m_timerMap.TryGetValue(id, out timer) && timer != null)
            {
                if (onRemoveTimer != null)
                {
                    onRemoveTimer(id);
                }
                timer.Release();
                m_timerMap.Remove(id);
            }
        }

        public void Start(int id, float time, Action actionTimeUp = null)
        {
            Timer timer = GetTimer(id, true);
            timer.Start(time, actionTimeUp);
        }

        public void Stop(int id)
        {
            Timer timer = GetTimer(id);
            if (timer != null)
            {
                timer.Stop();
                RemoveTimer(id);
            }
        }

        public void Pause(int id)
        {
            Timer timer = GetTimer(id);
            if (timer != null)
            {
                timer.Pause();
            }
        }

        public void Resume(int id)
        {
            Timer timer = GetTimer(id);
            if (timer != null)
            {
                timer.Resume();
            }
        }

        public void Finish(int id)
        {
            Timer timer = GetTimer(id);
            if (timer != null)
            {
                timer.Finish();
            }
        }

        public bool IsActive(int id)
        {
            Timer timer;
            return m_timerMap.TryGetValue(id, out timer) && timer != null && timer.isActive;
        }

        public float CurTime(int id)
        {
            Timer timer = GetTimer(id);
            if (timer != null)
            {
                return timer.curTime;
            }
            return 0f;
        }

        public float TarTime(int id)
        {
            Timer timer = GetTimer(id);
            if (timer != null)
            {
                return timer.targetTime;
            }
            return 0f;
        }

        public float LeftTime(int id)
        {
            Timer timer = GetTimer(id);
            if (timer != null)
            {
                return timer.leftTime;
            }
            return 0f;
        }

        public void Tick(float deltaTime)
        {
            foreach (var kv in m_timerMap)
            {
                m_timerList.Add(kv.Value);
            }
            int count = m_timerList.Count;
            for (int i = 0; i < count; ++i)
            {
                Timer timer = m_timerList[i];
                timer.Tick(deltaTime);
                if (timer.isFinished && !m_finishList.Contains(timer.id))
                {
                    m_finishList.Add(timer.id);
                }
            }
            m_timerList.Clear();
            if (m_finishList.Count > 0)
            {
                for (int i = 0; i < m_finishList.Count; ++i)
                {
                    if (onTimeUp != null)
                    {
                        onTimeUp(m_finishList[i]);
                    }
                    RemoveTimer(m_finishList[i]);
                }
                m_finishList.Clear();
            }
        }
    }
}