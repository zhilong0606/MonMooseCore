namespace MonMooseCore
{
    public abstract class ConditionDirty : ConditionBase
    {
        private bool m_result;
        private bool m_isDirty;

        public sealed override bool Check()
        {
            if (m_isDirty)
            {
                m_isDirty = false;
                m_result = GetResult();
            }
            return m_result;
        }

        protected void SetDirty()
        {
            m_isDirty = true;
        }

        protected abstract bool GetResult();

        public override void Reset()
        {
            base.Reset();
            m_isDirty = false;
            m_result = false;
        }
    }
}
