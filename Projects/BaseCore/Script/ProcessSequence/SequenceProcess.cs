using System.Collections.Generic;

namespace MonMooseCore
{
    public class SequenceProcess
    {
        private List<IProcessClip> processList = new List<IProcessClip>();
        private int processIndex;

        public IProcessClip CurProcess
        {
            get
            {
                if (processIndex < processList.Count)
                {
                    return processList[processIndex];
                }
                return null;
            }
        }

        public void Register(IProcessClip clip)
        {
            clip.Init();
            processList.Add(clip);
        }

        public void UnRegister(IProcessClip clip)
        {
            clip.UnInit();
            processList.Remove(clip);
        }

        public void Start()
        {
            for (int i = 0; i < processList.Count; ++i)
            {
                processList[i].OnEnd = OnProcessClipEnd;
            }
            processIndex = -1;
            ProcessNext();
        }

        public void Stop()
        {
            for (int i = 0; i < processList.Count; ++i)
            {
                processList[i].OnEnd = null;
                processList[i].OnResetClip();
            }
            processIndex = -1;
        }

        private void OnProcessClipEnd(IProcessClip clip)
        {
            if (clip == CurProcess)
            {
                ProcessNext();
            }
        }

        public void ProcessNext()
        {
            processIndex++;
            while (CurProcess != null)
            {
                if (CurProcess.CanStart)
                {
                    CurProcess.OnStartClip();
                    break;
                }
                CurProcess.OnSkipClip();
                processIndex++;
            }
        }
    }
}
