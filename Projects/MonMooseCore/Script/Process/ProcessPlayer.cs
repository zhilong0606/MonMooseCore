using System;
using System.Collections;
using System.Collections.Generic;

namespace MonMoose.Core
{
    public class ProcessPlayer
    {
        private List<ProcessBase> m_processList = new List<ProcessBase>();
        private ProcessBase m_curProcess;
        private bool m_isPaused;

        public Action actionOnEnd;

        public bool isStart
        {
            get { return m_curProcess != null; }
        }

        public void Start(ProcessBase process)
        {
            if (process == null)
            {
                return;
            }
            m_processList.Add(process);
            if (m_curProcess == null)
            {
                StartNext();
            }
        }

        public void Pause()
        {
            m_isPaused = true;
            if (m_curProcess != null)
            {
                m_curProcess.Pause();
            }
        }

        public void Resume()
        {
            if (!m_isPaused)
            {
                return;
            }
            m_isPaused = false;
            if (m_curProcess != null)
            {
                m_curProcess.Resume();
            }
            else
            {
                StartNext();
            }
        }

        private void OnProcessEnd(ProcessBase process)
        {
            process.UnInit();
            if (m_curProcess != process)
            {
                DebugUtility.LogError("Cannot End ProcessBase which is not Current Process. Please Remove it or Skip it:");
                return;
            }
            m_curProcess = null;
            StartNext();
        }
        
        public void Clear()
        {
            for (int i = 0; i < m_processList.Count; ++i)
            {
                m_processList[i].UnInit();
            }
            m_processList.Clear();
            if (m_curProcess != null)
            {
                m_curProcess.Release();
                m_curProcess = null;
            }
            m_isPaused = false;
        }

        private void StartNext()
        {
            while (m_processList.Count > 0)
            {
                ProcessBase process = m_processList[0];
                m_processList.RemoveAt(0);
                if (process.canStart)
                {
                    process.Start();
                    if (process.state != EProcessState.Ended)
                    {
                        m_curProcess = process;
                        m_curProcess.actionOnEnd = OnProcessEnd;
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
            if (m_processList.Count == 0 && m_curProcess == null)
            {
                actionOnEnd.InvokeSafely();
            }
        }

        public void Tick(float deltaTime)
        {
            if (m_curProcess != null)
            {
                m_curProcess.Tick(deltaTime);
            }
        }

        public void LogRunningProcess()
        {
            if (m_curProcess != null)
            {
                LogProcess(m_curProcess);
            }
        }

        private void LogProcess(ProcessBase process)
        {
            if (process is ProcessSequence)
            {
                LogSequenceProcess(process as ProcessSequence);
            }
            else if (process is ProcessParallel)
            {
                LogParallelProcess(process as ProcessParallel);
            }
            else
            {
                DebugUtility.LogError(string.Format("{0}ÕýÔÚÖ´ÐÐ", process.GetType().Name));
            }
        }

        private void LogSequenceProcess(ProcessSequence sequenceProcess)
        {
            if (sequenceProcess.curProcess != null)
            {
                LogProcess(sequenceProcess.curProcess);
            }
        }

        private void LogParallelProcess(ProcessParallel parallelProcess)
        {
            for (int i = 0; i < parallelProcess.subProcessList.Count; ++i)
            {
                LogProcess(parallelProcess.subProcessList[i]);
            }
        }
    }
}
