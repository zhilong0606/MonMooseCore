using System;

namespace MonMoose.Core
{
    public abstract class TriggerBase
    {
        private int m_id;
        private int m_typeId;
        private TriggerConditionBase m_condition;
        private TriggerActionBase m_action;
        private Action m_actionOnTriggerOvered;

        public int id { get { return m_id; } }

        public int typeId
        {
            get { return m_typeId; }
        }

        public void Start()
        {
            OnStart();
        }

        public void Stop()
        {
            OnStop();
        }

        public void TryTrigger()
        {
            if (CheckCondition())
            {
                Trigger();
            }
        }

        public void Trigger()
        {
            if (m_action != null)
            {
                m_action.Invoke();
            }
            m_actionOnTriggerOvered.InvokeSafely();
            m_actionOnTriggerOvered = null;
        }

        protected bool CheckCondition()
        {
            return OnCheckCondition() && (m_condition == null || m_condition.Check());
        }

        protected virtual void OnStart() { }
        protected virtual void OnStop() { }
        protected virtual bool OnCheckCondition() { return true; }
    }
}
