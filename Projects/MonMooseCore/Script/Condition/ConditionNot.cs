namespace MonMoose.Core
{
    public sealed class ConditionNot : ConditionBase
    {
        private ConditionBase condition;

        public override void OnRelease()
        {
            condition = default;
            base.OnRelease();
        }

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