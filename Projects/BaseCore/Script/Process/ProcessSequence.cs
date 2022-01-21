namespace MonMooseCore
{
    public class ProcessSequence : ProcessCollection
    {
        private ProcessBase m_curProcess;

        public ProcessBase curProcess
        {
            get { return m_curProcess; }
        }

        protected override void OnStart()
        {
            ProcessNext();
        }

        protected override void OnSubProcessEnd(ProcessBase process)
        {
            if (m_curProcess != process)
            {
                DebugUtility.LogError("Cannot End Process which is not Current Process. Please Remove it or Skip it.");
                return;
            }
            process.UnInit();
            ProcessNext();
        }

        public void ProcessNext()
        {
            m_curProcess = null;
            if (m_subProcessList.Count > 0)
            {
                while (m_subProcessList.Count > 0)
                {
                    ProcessBase process = m_subProcessList[0];
                    m_subProcessList.RemoveAt(0);
                    if (process.canStart)
                    {
                        process.Start();
                        if (process.state != EProcessState.Ended)
                        {
                            m_curProcess = process;
                            m_curProcess.actionOnEnd = OnSubProcessEnd;
                            break;
                        }
                        else
                        {
                            process.UnInit();
                        }
                    }
                    else
                    {
                        process.Skip();
                        process.UnInit();
                    }
                }
            }
            if (m_subProcessList.Count == 0 && m_curProcess == null)
            {
                End();
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            if (m_curProcess != null)
            {
                m_curProcess.Pause();
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (m_curProcess != null)
            {
                m_curProcess.Resume();
            }
        }

        protected override void OnUpdate(float deltaTime)
        {
            if (m_curProcess != null)
            {
                m_curProcess.Update(deltaTime);
            }
        }

        public override void OnRelease()
        {
            m_curProcess = null;
            base.OnRelease();
        }
    }
}