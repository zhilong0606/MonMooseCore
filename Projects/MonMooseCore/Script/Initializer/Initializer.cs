using System;
using System.Collections;
using System.Collections.Generic;

namespace MonMooseCore
{
    public abstract class Initializer
    {
        private List<Initializer> m_subList = new List<Initializer>();
        private IEnumerator m_process;
        private Action<Initializer> m_actionOnFinish;
        private EInitialType m_initialType = EInitialType.Async;
        private EState m_state = EState.None;
        private int m_curStep;
        private int m_curSubIndex;
        private bool m_isRoot;

        protected virtual int maxStep { get { return 2; } }

        public float processRate
        {
            get
            {
                float rate = Clamp01((float)m_curSubIndex / (m_subList.Count + 1));
                float subRate = 0f;
                if (m_curSubIndex <= 0)
                {
                    subRate = Clamp01((float)m_curStep / maxStep);
                }
                else if (m_curSubIndex <= m_subList.Count)
                {
                    subRate = m_subList[m_curSubIndex - 1].processRate;
                }
                return rate + subRate / (m_subList.Count + 1);
            }
        }

        public bool IsFinished
        {
            get { return m_state == EState.Finish; }
        }

        public bool NeedRegisterTick
        {
            get { return m_isRoot && m_initialType == EInitialType.Async; }
        }

        public Initializer(params Initializer[] array)
        {
            if (array != null && array.Length > 0)
            {
                m_subList.AddRange(array);
            }
        }

        public void AddSubInitializer(Initializer sub)
        {
            m_subList.Add(sub);
        }

        public void StartSync()
        {
            m_isRoot = true;
            StartInternal(EInitialType.Sync, null);
        }

        public void StartAsync(Action<Initializer> actionOnFinish)
        {
            m_isRoot = true;
            StartInternal(EInitialType.Async, actionOnFinish);
        }

        private bool StartInternal(EInitialType initialType, Action<Initializer> actionOnFinish)
        {
            if (m_state == EState.Process)
            {
                return false;
            }
            if (m_state == EState.Finish)
            {
                return false;
            }
            m_initialType = initialType;
            m_state = EState.Process;
            m_actionOnFinish = actionOnFinish;
            m_process = OnProcess();
            if (m_process == null)
            {
                return false;
            }
            m_curStep = 0;
            if (NeedRegisterTick)
            {
                TickManager.instance.RegisterGlobalTick(Tick);
            }
            InitializeInternal();
            for (int i = 0; i < m_subList.Count; ++i)
            {
                m_subList[i].StartInternal(m_initialType, null);
            }
            if (m_initialType == EInitialType.Sync)
            {
                Finish();
            }
            return true;
        }

        public void Reset()
        {
            m_isRoot = false;
            m_state = EState.None;
            for (int i = 0; i < m_subList.Count; ++i)
            {
                m_subList[i].Reset();
            }
        }

        public void Finish()
        {
            OnFinish();
            if (NeedRegisterTick)
            {
                TickManager.instance.UnRegisterGlobalTick(Tick);
            }
            if (m_actionOnFinish != null)
            {
                m_actionOnFinish(this);
            }
            m_state = EState.Finish;
        }

        private void InitializeInternal()
        {
            if (m_initialType == EInitialType.Sync)
            {
                while (m_process.MoveNext())
                {
                    m_curStep++;
                }
                m_curSubIndex++;
            }
        }

        public void Tick(float deltaTime)
        {
            if (m_initialType == EInitialType.Async)
            {
                if (m_curSubIndex <= 0)
                {
                    bool isMainProcessResult;
                    bool isContinued = MoveNext(m_process, out isMainProcessResult);
                    if (isMainProcessResult)
                    {
                        m_curStep++;
                    }
                    if (!isContinued)
                    {
                        m_process = null;
                        m_curSubIndex++;
                    }
                }
                else if (m_curSubIndex <= m_subList.Count)
                {
                    m_subList[m_curSubIndex - 1].Tick(deltaTime);
                    if (m_subList[m_curSubIndex - 1].IsFinished)
                    {
                        m_curSubIndex++;
                    }
                }
                if (m_curSubIndex > m_subList.Count)
                {
                    Finish();
                }
            }
        }

        private bool MoveNext(IEnumerator iter, out bool isMainProcessResult)
        {
            isMainProcessResult = true;
            if (iter == null)
            {
                return false;
            }
            if (iter.Current != null)
            {
                IEnumerator subIter = iter.Current as IEnumerator;
                bool b;
                if (MoveNext(subIter, out b))
                {
                    isMainProcessResult = false;
                    return true;
                }
            }
            return iter.MoveNext();
        }

        private float Clamp01(float f)
        {
            if (f < 0f)
                return 0f;
            if (f > 1f)
                return 0f;
            return f;
        }

        protected virtual void OnFinish() { }
        protected abstract IEnumerator OnProcess();

        public enum EInitialType
        {
            Sync,
            Async,
        }

        public enum EState
        {
            None,
            Process,
            Finish,
        }
    }

}