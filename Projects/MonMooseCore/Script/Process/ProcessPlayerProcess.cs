using System;
using System.Collections;
using System.Collections.Generic;

namespace MonMoose.Core
{
	public abstract class ProcessPlayerProcess : ProcessBase
    {
        private SequenceProcess m_process;

        public override void OnRelease()
        {
            m_process.Release();
            m_process = null;
            base.OnRelease();
        }

        protected override void OnInit()
        {
            base.OnInit();
            m_process = ClassPoolManager.instance.Fetch<SequenceProcess>();
            m_process.checkPointId = checkPointId;
            OnInitProcess(m_process);
        }

        protected sealed override void OnStart()
        {
            if (m_process.InitAndStartAndCheckRunning())
            {
                m_process.actionOnEnd = OnSubProcessEnd;
            }
            else
            {
                m_process.UnInit();
                End();
            }
        }

        protected abstract void OnInitProcess(SequenceProcess sequenceProcess);

        protected override void OnPause()
        {
            m_process.Pause();
        }

        protected override void OnResume()
        {
            m_process.Resume();
        }

        protected override void OnTick(TimeSlice timeSlice) 
        {
            m_process.Tick(timeSlice);
        }

        private void OnSubProcessEnd(ProcessBase process)
        {
            m_process.UnInit();
            End();
        }
    }
}
