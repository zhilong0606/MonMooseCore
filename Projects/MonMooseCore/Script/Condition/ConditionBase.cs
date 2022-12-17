namespace MonMoose.Core
{
    public abstract class ConditionBase : ClassPoolObj
    {
        protected ConditionBase m_parent;

        public ConditionBase parent
        {
            get { return m_parent; }
        }

        public abstract bool Check();

        public virtual void Reset()
        {
            m_parent = null;
        }

        public sealed override void OnRelease()
        {
            Reset();
        }

        protected void Attach(ConditionBase child)
        {
            child.m_parent = this;
        }

        public static ConditionOr operator |(ConditionBase c1, ConditionBase c2)
        {
            ConditionOr c = ClassPoolManager.instance.Fetch<ConditionOr>();
            c.checkPointId = c1.checkPointId;
            c.Init(c1, c2);
            return c;
        }

        public static ConditionAnd operator &(ConditionBase c1, ConditionBase c2)
        {
            ConditionAnd c = ClassPoolManager.instance.Fetch<ConditionAnd>();
            c.checkPointId = c1.checkPointId;
            c.Init(c1, c2);
            return c;
        }

        public static ConditionNot operator !(ConditionBase c1)
        {
            ConditionNot c = ClassPoolManager.instance.Fetch<ConditionNot>();
            c.checkPointId = c1.checkPointId;
            c.Init(c1);
            return c;
        }
    }
}
