using System;

namespace MonMooseCore
{
    public abstract class AbstractStaticLerp
    {
        protected float m_curTime;
        protected float m_lerpTime;
        protected bool m_isStart;
        protected ILerpFunc m_lerpFunc = LerpUtility.GetBaseFunc(EBaseLerpFuncType.Linear);

        public bool isStart
        {
            get { return m_isStart; }
        }

        public float curTime
        {
            get { return m_curTime; }
            set
            {
                m_curTime = value;
                CalcValueAndSendTick();
            }
        }

        public float normalizedTime
        {
            get { return Clamp01(Math.Abs(m_lerpTime) < float.Epsilon ? 0 : m_curTime / m_lerpTime); }
            set { curTime = Clamp01(value) * m_lerpTime; }
        }

        public bool needFinish
        {
            get { return m_curTime > m_lerpTime; }
        }

        public void Start()
        {
            m_isStart = true;
        }

        public void Stop()
        {
            m_isStart = false;
        }

        public void Finish()
        {
            m_curTime = m_lerpTime;
            Stop();
            OnFinish();
        }

        protected float Clamp01(float f)
        {
            if (f < 0f)
                return 0f;
            if (f > 1f)
                return 1f;
            return f;
        }

        protected abstract void CalcValueAndSendTick();
        protected abstract void OnFinish();
        public abstract void Tick(float deltaTime);
    }

    public abstract class AbstractStaticLerp<T> : AbstractStaticLerp
    {
        protected T m_targetValue;
        protected T m_startValue;
        protected T m_curValue;
        protected T m_preValue;
        public Action<AbstractStaticLerp<T>> actionOnFinish;
        public Action<AbstractStaticLerp<T>> actionOnTick;

        public T curValue
        {
            get { return m_curValue; }
            set { m_curValue = value; }
        }

        public T preValue
        {
            get { return m_preValue; }
            set { m_preValue = value; }
        }

        public T startValue
        {
            get { return m_startValue; }
        }

        public T targetValue
        {
            get { return m_targetValue; }
        }

        public void Start(T startValue, T targetValue, float time, EBaseLerpFuncType baseFuncType)
        {
            Ready(startValue, targetValue, time, baseFuncType);
            m_isStart = true;
        }

        public void Ready(T startValue, T targetValue, float time, EBaseLerpFuncType baseFuncType)
        {
            m_lerpFunc = LerpUtility.GetBaseFunc(baseFuncType);
            Ready(startValue, targetValue, time);
        }

        public void Start(T startValue, T targetValue, float time, int custemFuncType)
        {
            Ready(startValue, targetValue, time, custemFuncType);
            m_isStart = true;
        }

        public void Ready(T startValue, T targetValue, float time, int custemFuncType)
        {
            m_lerpFunc = LerpUtility.GetCustomFunc(custemFuncType);
            Ready(startValue, targetValue, time);
        }

        public void Ready(T startValue, T targetValue, float time)
        {
            m_targetValue = targetValue;
            if (!m_isStart)
            {
                m_startValue = startValue;
                m_curValue = startValue;
                m_preValue = startValue;
            }
            else
            {
                m_startValue = m_curValue;
            }
            m_lerpTime = time;
            Reset();
        }

        public void Reset()
        {
            m_curTime = 0f;
            CalcValueAndSendTick();
            Stop();
        }

        protected override void OnFinish()
        {
            m_preValue = m_curValue;
            m_curValue = m_targetValue;
            if (actionOnTick != null)
            {
                actionOnTick(this);
            }
            if (actionOnFinish != null)
            {
                actionOnFinish(this);
            }
        }

        protected override void CalcValueAndSendTick()
        {
            m_preValue = m_curValue;
            float f = m_lerpFunc.GetValue(normalizedTime);
            m_curValue = Lerp(m_startValue, m_targetValue, f);
            if (actionOnTick != null)
            {
                actionOnTick(this);
            }
        }

        public override void Tick(float deltaTime)
        {
            if (m_isStart)
            {
                m_curTime += deltaTime;
                if (needFinish)
                {
                    Finish();
                }
                else
                {
                    CalcValueAndSendTick();
                }
            }
        }

        protected abstract T Lerp(T start, T end, float f);
    }
}
