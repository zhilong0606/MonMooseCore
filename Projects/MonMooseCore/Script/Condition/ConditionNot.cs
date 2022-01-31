namespace MonMooseCore
{
    public sealed class ConditionNot : ConditionBase
    {
        private ConditionBase condition;

        public void Init(ConditionBase c)
        {
            condition = c;
            Attach(c);
        }

        public override bool Check()
        {
            return !condition.Check();
        }

        public override void Reset()
        {
            condition = null;
            base.Reset();
        }
    }
}