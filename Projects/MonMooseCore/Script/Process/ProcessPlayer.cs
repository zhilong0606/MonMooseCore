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
            process.Release();
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
            m_processList.ReleaseAll();
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
                if (process.InitAndStartAndCheckRunning())
                {
                    process.actionOnEnd = OnProcessEnd;
                    m_curProcess = process;
                    break;
                }
                process.UnInit();
                process.Release();
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
            if (process is SequenceProcess)
            {
                LogSequenceProcess(process as SequenceProcess);
            }
            else if (process is ParallelProcess)
            {
                LogParallelProcess(process as ParallelProcess);
            }
            else
            {
                DebugUtility.LogError(string.Format("{0}ÕýÔÚÖ´ÐÐ", process.GetType().Name));
            }
        }

        private void LogSequenceProcess(SequenceProcess sequenceProcess)
        {
            if (sequenceProcess.curProcess != null)
            {
                LogProcess(sequenceProcess.curProcess);
            }
        }

        private void LogParallelProcess(ParallelProcess parallelProcess)
        {
            for (int i = 0; i < parallelProcess.subProcessList.Count; ++i)
            {
                LogProcess(parallelProcess.subProcessList[i]);
            }
        }
    }
}
