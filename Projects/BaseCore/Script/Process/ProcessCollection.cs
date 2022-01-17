using System;
using System.Collections.Generic;

namespace MonMooseCore
{
    public abstract class ProcessCollection : ProcessBase
    {
        protected List<ProcessBase> m_subProcessList = new List<ProcessBase>();

        public Func<bool> funcOnCanStart;
        public Action actionOnSkip;

        public List<ProcessBase> subProcessList
        {
            get { return m_subProcessList; }
        }

        public override bool canStart
        {
            get { return m_subProcessList.Count > 0 && (funcOnCanStart == null || funcOnCanStart()); }
        }

        protected override void OnInit()
        {
            for (int i = 0; i < m_subProcessList.Count; ++i)
            {
                m_subProcessList[i].Init();
            }
        }

        public void Add(ProcessBase process)
        {
            if (process == null)
            {
                return;
            }
            process.actionOnEnd = OnSubProcessEnd;
            m_subProcessList.Add(process);
        }

        protected override void OnSkip()
        {
            if (actionOnSkip != null)
            {
                actionOnSkip();
            }
            base.OnSkip();
        }

        public override void OnRelease()
        {
            for (int i = 0; i < m_subProcessList.Count; ++i)
            {
                m_subProcessList[i].UnInit();
            }
            m_subProcessList.Clear();
            actionOnSkip = null;
            funcOnCanStart = null;
            base.OnRelease();
        }

        protected abstract void OnSubProcessEnd(ProcessBase process);
    }
}
