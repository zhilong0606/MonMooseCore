using System;
using System.Collections;
using System.Collections.Generic;

namespace MonMoose.Core
{
    public class ProcessParallel : ProcessCollection
    {
        private List<ProcessBase> m_tempList = new List<ProcessBase>();

        protected override void OnStart()
        {
            if (m_subProcessList.Count > 0)
            {
                m_tempList.AddRange(m_subProcessList);
                for (int i = 0; i < m_tempList.Count; ++i)
                {
                    ProcessBase process = m_tempList[i];
                    process.Init();
                    if (process.canStart)
                    {
                        process.Start();
                        if (process.isStarted)
                        {
                            process.actionOnEnd = OnSubProcessEnd;
                        }
                        else
                        {
                            OnSubProcessEnd(process);
                        }
                    }
                    else
                    {
                        process.Skip();
                        OnSubProcessEnd(process);
                    }
                }
                m_tempList.Clear();
            }
            else
            {
                End();
            }
        }

        protected override void OnTick(float deltaTime)
        {
            m_tempList.AddRange(m_subProcessList);
            for (int i = 0; i < m_tempList.Count; ++i)
            {
                ProcessBase process = m_tempList[i];
                if (process.state == EProcessState.Started)
                {
                    process.Tick(deltaTime);
                }
            }
            m_tempList.Clear();
        }

        protected override void OnPause()
        {
            int count = m_subProcessList.Count;
            for (int i = 0; i < count; ++i)
            {
                m_subProcessList[i].Pause();
            }
        }

        protected override void OnResume()
        {
            int count = m_subProcessList.Count;
            for (int i = 0; i < count; ++i)
            {
                m_subProcessList[i].Resume();
            }
        }

        public override void OnRelease()
        {
            m_tempList.Clear();
            base.OnRelease();
        }

        private void OnSubProcessEnd(ProcessBase process)
        {
            process.UnInit();
            for (int i = 0; i < m_subProcessList.Count; ++i)
            {
                if (m_subProcessList[i].state != EProcessState.UnInited)
                {
                    return;
                }
            }
            End();
        }
    }
}
