using System;

namespace MonMooseCore
{
    public class Timer : ClassPoolObj
    {
        private float m_targetTime;
        private float m_curTime;
        private Action m_actionTimeUp;

        public int id { get; set; }
        public bool isActive { get; private set; }
        public bool isFinished { get; private set; }

        public float curTime
        {
            get { return m_curTime; }
        }

        public float targetTime
        {
            get { return m_targetTime; }
        }

        public float leftTime
        {
            get { return m_targetTime - m_curTime; }
        }

        public void Tick(float deltaTime)
        {
            if (isActive)
            {
                m_curTime += deltaTime;
                if (m_curTime >= m_targetTime)
                {
                    m_curTime = m_targetTime;
                    Finish();
                }
            }
        }

        public void Start(float time, Action actionTimeUp = null)
        {
            isFinished = false;
            isActive = true;
            m_curTime = 0f;
            m_targetTime = time;
            m_actionTimeUp = actionTimeUp;
        }

        public void Stop()
        {
            isActive = false;
            isFinished = true;
        }

        public void Pause()
        {
            isActive = false;
        }

        public void Resume()
        {
            isActive = true;
        }

        public void Finish()
        {
            isActive = false;
            isFinished = true;
            if (m_actionTimeUp != null && m_actionTimeUp.Target != null)
            {
                m_actionTimeUp();
            }
        }

        public override void OnRelease()
        {
            m_curTime = 0f;
            isFinished = true;
            isActive = false;
            m_actionTimeUp = null;
        }
    }
}