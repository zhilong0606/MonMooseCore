namespace MonMooseCore
{
    public sealed class ConditionAnd : ConditionBaseTwo
    {
        public override bool Check()
        {
            return condition1.Check() && condition2.Check();
        }
    }
}