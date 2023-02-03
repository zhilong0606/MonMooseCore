using System;
using System.Collections;
using System.Collections.Generic;

namespace MonMoose.Core
{
    public abstract class CollectionProcess<T> : ProcessBase where T : ProcessBase
    {
        protected List<T> m_subProcessList = new List<T>();

        public Func<bool> funcOnCanStart;
        public Action actionOnSkip;

        public List<T> subProcessList
        {
            get { return m_subProcessList; }
        }

        public override bool canStart
        {
            get { return m_subProcessList.Count > 0 && (funcOnCanStart == null || funcOnCanStart()); }
        }

        public override void OnRelease()
        {
            m_subProcessList.ReleaseAll();
            m_subProcessList.Clear();
            base.OnRelease();
        }

        public void AddProcess(T process)
        {
            if (process == null)
            {
                return;
            }
            if (m_subProcessList.Contains(process))
            {
                return;
            }
            m_subProcessList.Add(process);
        }

        protected bool StartSubProcessAndCheckRunning(ProcessBase process)
        {
            if (process != null)
            {
                if (process.InitAndStartAndCheckRunning())
                {
                    process.actionOnEnd = OnSubProcessEnd;
                    return true;
                }
            }
            return false;
        }

        protected override void OnSkip()
        {
            if (actionOnSkip != null)
            {
                actionOnSkip();
            }
            base.OnSkip();
        }

        protected override void OnUnInit()
        {
            funcOnCanStart = null;
            actionOnSkip = null;
        }

        protected virtual void OnSubProcessEnd(ProcessBase process)
        {

        }
    }
}
