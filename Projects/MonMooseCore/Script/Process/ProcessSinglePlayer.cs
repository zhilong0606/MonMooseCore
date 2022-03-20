using System;
using System.Collections;
using System.Collections.Generic;

namespace MonMoose.Core
{
	public class ProcessSinglePlayer : ProcessBase
    {
        private ProcessBase m_process;

        public override bool canStart
        {
            get { return m_process != null; }
        }

        public void SetProcess(ProcessBase process)
        {
            m_process = process;
        }

        protected override void OnStart()
        {
            m_process.Init();
            m_process.Start();

            if (m_process.canStart)
            {
                m_process.Start();
                if (m_process.isStarted)
                {
                    m_process.actionOnEnd = OnSubProcessEnd;
                }
                else
                {
                    m_process.UnInit();
                    End();
                }
            }
            else
            {
                m_process.Skip();
                m_process.UnInit();
                End();
            }
        }

        protected override void OnPause()
        {
            m_process.Pause();
        }

        protected override void OnResume()
        {
            m_process.Resume();
        }

        protected override void OnTick(float deltaTime)
        {
            m_process.Tick(deltaTime);
        }

        public override void OnRelease()
        {
            if (m_process != null)
            {
                m_process.Release();
            }
            m_process = null;
            base.OnRelease();
        }

        private void OnSubProcessEnd(ProcessBase process)
        {
            m_process.UnInit();
            End();
        }
    }
}
