using System;
using System.Collections;
using System.Collections.Generic;

namespace MonMoose.Core
{
    public abstract class Command : ClassPoolObj
    {
        private Action<Command> m_actionOnEnd;
        private StateId m_stateId;

        public override void OnRelease()
        {
            m_stateId = StateId.None;
            m_actionOnEnd = null;
            base.OnRelease();
        }

        public void Execute(Action<Command> actionOnEnd)
        {
            if (m_stateId == StateId.None)
            {
                m_stateId = StateId.Executing;
                m_actionOnEnd = actionOnEnd;
                OnExecute();
            }
            else
            {
                DebugUtility.LogError("[Command] Execute: {0} must be State Of None");
            }
        }

        public void End()
        {
            if (m_stateId == StateId.Executing)
            {
                m_stateId = StateId.End;
                OnEnd();
                NotifyEnd();
            }
            else
            {
                DebugUtility.LogError("[Command] End: {0} must be State Of Executing");
            }
        }

        private void NotifyEnd()
        {
            if (m_actionOnEnd != null)
            {
                Action<Command> temp = m_actionOnEnd;
                m_actionOnEnd = null;
                temp(this);
            }
        }

        protected virtual void OnEnd() { }

        protected abstract void OnExecute();

        private enum StateId
        {
            None,
            Executing,
            End,
        }
    }
}
