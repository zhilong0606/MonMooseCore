using System;

namespace MonMoose.Core
{
    public class Task : ClassPoolObj
    {
        public Action<Task> actionOnEnd;
        public Action<Task> actionOnRemove;
        private EState m_state = EState.None;

        public EState state
        {
            get { return m_state; }
        }

        public override void OnRelease()
        {
            m_state = EState.None;
            actionOnEnd = null;
            actionOnRemove = null;
            base.OnRelease();
        }

        public void Start()
        {
            TaskManager.instance.AddTask(this);
            m_state = EState.Start;
            OnStart();
        }

        public void End()
        {
            OnEnd();
            m_state = EState.End;
            if (actionOnEnd != null)
            {
                actionOnEnd(this);
            }
        }

        public void Remove()
        {
            if (actionOnRemove != null)
            {
                actionOnRemove(this);
            }
        }

        public void Tick(TimeSlice timeSlice)
        {
            if (m_state == EState.Start)
            {
                OnTick(timeSlice);
            }
        }

        protected virtual void OnStart()
        {

        }

        protected virtual void OnEnd()
        {

        }

        protected virtual void OnTick(TimeSlice timeSlice)
        {

        }

        public enum EState
        {
            None,
            Start,
            End,
        }
    }
}
