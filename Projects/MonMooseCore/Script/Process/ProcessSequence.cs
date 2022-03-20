using System;
using System.Collections;
using System.Collections.Generic;

namespace MonMoose.Core
{
    public class ProcessSequence : ProcessCollection
    {
        private int m_curIndex;

        public ProcessBase curProcess
        {
            get { return m_subProcessList.GetValueSafely(m_curIndex); }
        }

        public void ProcessNext()
        {
            while (m_curIndex < m_subProcessList.Count)
            {
                m_curIndex++;
                ProcessBase process = m_subProcessList.GetValueSafely(m_curIndex);
                if (process == null)
                {
                    break;
                }
                process.Init();
                if (process.canStart)
                {
                    process.Start();
                    if (process.isStarted)
                    {
                        process.actionOnEnd = OnSubProcessEnd;
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
            if (curProcess == null)
            {
                End();
            }
        }

        protected override void OnStart()
        {
            ProcessNext();
        }

        protected override void OnPause()
        {
            if (curProcess != null)
            {
                curProcess.Pause();
            }
        }

        protected override void OnResume()
        {
            if (curProcess != null)
            {
                curProcess.Resume();
            }
        }

        protected override void OnTick(float deltaTime)
        {
            if (curProcess != null)
            {
                curProcess.Tick(deltaTime);
            }
        }

        public override void OnRelease()
        {
            m_curIndex = 0;
            base.OnRelease();
        }

        private void OnSubProcessEnd(ProcessBase process)
        {
            int index = m_subProcessList.IndexOf(process);
            if (m_curIndex != index)
            {
                DebugUtility.LogError("Cannot End Process which is not Current Process. Please Remove it or Skip it.");
                return;
            }
            process.UnInit();
            ProcessNext();
        }
    }
}
