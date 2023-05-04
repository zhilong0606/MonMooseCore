using System;
using System.Collections.Generic;

namespace MonMoose.Core
{
    public class TimerManager : Singleton<TimerManager>
    {
        private TimerMap m_timerMap = new TimerMap();
        private List<int> m_activeIdList = new List<int>();
        private int m_idCursor;

        protected override void OnInit()
        {
            m_timerMap.onAddTimer = OnAddTimer;
            m_timerMap.onRemoveTimer = OnRemoveTimer;
            m_timerMap.pool = ClassPoolManager.instance.GetPool(typeof(Timer));
            TickManager.instance.RegisterGlobalTick(Tick);
            base.OnInit();
        }

        private void OnAddTimer(int id)
        {
            m_activeIdList.Add(id);
        }

        private void OnRemoveTimer(int id)
        {
            m_activeIdList.Remove(id);
        }

        private int GetValidId()
        {
            do
            {
                m_idCursor++;
                if (m_idCursor == int.MaxValue)
                {
                    m_idCursor = 1;
                }
            } while (m_activeIdList.Contains(m_idCursor));
            return m_idCursor;
        }

        public void Tick(TimeSlice timeSlice)
        {
            m_timerMap.Tick(timeSlice);
        }

        public Timer GetTimer(int id)
        {
            return m_timerMap.GetTimer(id);
        }

        public int Start(float time, Action actionTimeUp = null)
        {
            int id = GetValidId();
            m_timerMap.Start(id, time, actionTimeUp);
            return id;
        }

        public void Stop(int id)
        {
            m_timerMap.Stop(id);
        }

        public void Stop(ref int id)
        {
            m_timerMap.Stop(id);
            id = 0;
        }

        public void Pause(int id)
        {
            m_timerMap.Pause(id);
        }

        public void Resume(int id)
        {
            m_timerMap.Resume(id);
        }

        public void Finish(int id)
        {
            m_timerMap.Finish(id);
        }

        public bool IsActive(int id)
        {
            return m_timerMap.IsActive(id);
        }

        public float CurTime(int id)
        {
            return m_timerMap.CurTime(id);
        }

        public float TarTime(int id)
        {
            return m_timerMap.TarTime(id);
        }

        public float LeftTime(int id)
        {
            return m_timerMap.LeftTime(id);
        }
    }
}
