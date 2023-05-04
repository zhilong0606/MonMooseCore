using System;
using System.Collections;
using System.Collections.Generic;

namespace MonMoose.Core
{
    public class ParallelProcess : CollectionProcess<ProcessBase>
    {
        protected override void OnStart()
        {
            if (m_subProcessList.Count > 0)
            {
                for (int i = 0; i < m_subProcessList.Count; ++i)
                {
                    ProcessBase process = m_subProcessList[i];
                    if (process == null)
                    {
                        continue;
                    }
                    if (StartSubProcessAndCheckRunning(process))
                    {
                        break;
                    }
                    OnSubProcessEnd(process);
                }
            }
            else
            {
                End();
            }
        }

        protected override void OnTick(TimeSlice timeSlice)
        {
            for (int i = 0; i < m_subProcessList.Count; ++i)
            {
                ProcessBase process = m_subProcessList[i];
                if (process.state == ProcessStateId.Started)
                {
                    process.Tick(timeSlice);
                }
            }
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

        protected override void OnSubProcessEnd(ProcessBase process)
        {
            process.UnInit();
            for (int i = 0; i < m_subProcessList.Count; ++i)
            {
                if (m_subProcessList[i].state != ProcessStateId.UnInited)
                {
                    return;
                }
            }
            End();
        }
    }
}
