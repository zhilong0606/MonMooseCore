using System;
using System.Collections;
using System.Collections.Generic;

namespace MonMoose.Core
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

        protected override void OnUnInit()
        {
            funcOnCanStart = null;
            actionOnSkip = null;
        }

        public void Add(ProcessBase process)
        {
            if (process == null)
            {
                return;
            }
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
            m_subProcessList.ReleaseAll();
            m_subProcessList.Clear();
            base.OnRelease();
        }
    }
}
