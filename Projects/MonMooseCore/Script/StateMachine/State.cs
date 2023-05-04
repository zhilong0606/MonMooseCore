namespace MonMoose.Core
{
    public abstract class State
    {
        protected StateMachine m_ownerMachine;
        protected bool m_isInited;
        protected bool m_isRunning;

        public abstract int stateIndex { get; }
        public StateMachine ownerMachine { get { return m_ownerMachine; } }

        public void Init(StateMachine ownerMachine)
        {
            m_ownerMachine = ownerMachine;
            m_isInited = true;
            OnInit();
        }

        public void UnInit()
        {
            OnUnInit();
            m_isInited = false;
        }

        public void Enter(StateContext context)
        {
            m_isRunning = true;
            OnEnter(context);
            if (context != null)
            {
                context.Release();
            }
        }

        public void Exit()
        {
            OnExit();
            m_isRunning = false;
        }

        public void Reset()
        {
            OnReset();
            m_isRunning = false;
        }

        public void Tick(TimeSlice timeSlice)
        {
            OnTick(timeSlice);
        }

        protected virtual void OnInit() { }
        protected virtual void OnUnInit() { }
        protected virtual void OnEnter(StateContext context) { }
        protected virtual void OnExit() { }
        protected virtual void OnTick(TimeSlice timeSlice) { }
        protected virtual void OnReset() { }
    }
}