namespace MonMooseCore
{
    public abstract class ClassPoolObj : IClassPoolObj
    {
        public ClassPool creator { get; set; }
        public object causer { get; set; }

        public virtual void OnFetch()
        {
        }

        public virtual void OnRelease()
        {
        }
    }
}
