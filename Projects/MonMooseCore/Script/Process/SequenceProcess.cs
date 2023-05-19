using System;
using System.Collections;
using System.Collections.Generic;

namespace MonMoose.Core
{
    public class SequenceProcess : CollectionProcess<ProcessBase>
    {
        private int m_curIndex;

        public ProcessBase curProcess
        {
            get { return m_subProcessList.GetValueSafely(m_curIndex); }
        }

        public override void OnRelease()
        {
            m_curIndex = default;
            base.OnRelease();
        }

        public void ProcessNext()
        {
            while (m_curIndex < m_subProcessList.Count - 1)
            {
                m_curIndex++;
                ProcessBase process = m_subProcessList.GetValueSafely(m_curIndex);
                if (process == null)
                {
                    continue;
                }
                if (StartSubProcessAndCheckRunning(process))
                {
                    break;
                }
                process.UnInit();
            }
            if (curProcess == null)
            {
                End();
            }
        }

        public override void OnFetch()
        {
            m_curIndex = -1;
            base.OnFetch();
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

        protected override void OnTick(TimeSlice timeSlice)
        {
            if (curProcess != null)
            {
                curProcess.Tick(timeSlice);
            }
        }

        protected override void OnSubProcessEnd(ProcessBase process)
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
