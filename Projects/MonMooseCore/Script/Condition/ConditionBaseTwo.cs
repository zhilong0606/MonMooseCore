namespace MonMoose.Core
{
    public abstract class ConditionBaseTwo : ConditionBase
    {
        protected ConditionBase condition1;
        protected ConditionBase condition2;

        public override void OnRelease()
        {
            condition1 = default;
            condition2 = default;
            base.OnRelease();
        }

        public void Init(ConditionBase c1, ConditionBase c2)
        {
            condition1 = c1;
            condition2 = c2;
            Attach(c1);
            Attach(c2);
        }

        public override void Reset()
        {
            condition1 = null;
            condition2 = null;
            base.Reset();
        }
    }
}