using System.Collections.Generic;

namespace MonMooseCore
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
                    if (process.canStart)
                    {
                        process.Start();
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

        protected override void OnSubProcessEnd(ProcessBase process)
        {
            process.UnInit();
            m_subProcessList.Remove(process);
            if (m_subProcessList.Count == 0)
            {
                End();
            }
        }

        protected override void OnUpdate(float deltaTime)
        {
            m_tempList.AddRange(m_subProcessList);
            for (int i = 0; i < m_tempList.Count; ++i)
            {
                ProcessBase process = m_tempList[i];
                if (process.state == EProcessState.Started)
                {
                    process.Update(deltaTime);
                }
            }
            m_tempList.Clear();
        }

        protected override void OnPause()
        {
            base.OnPause();
            int count = m_subProcessList.Count;
            for (int i = 0; i < count; ++i)
            {
                m_subProcessList[i].Pause();
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
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
    }
}
