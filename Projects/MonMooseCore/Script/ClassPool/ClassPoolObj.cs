namespace MonMoose.Core
{
    public abstract class ClassPoolObj : IClassPoolObj
    {
        public ClassPool creator { get; set; }
        public int checkPointId { get; set; }
        public object causer { get; set; }
        public System.Diagnostics.StackTrace stackTrace { get; set; }
        public string createLog { get; set; }

        public ClassPoolObj()
        {
#if !RELEASE && DEBUG_CLASSPOOL
            if (!new System.Diagnostics.StackTrace().ToString().Contains("ClassPool.CreateNew"))
            {
                DebugUtility.LogError(string.Format("[ClassPoolObj] Instance of {0} is not created by a pool", GetType().Name));
            }
#endif
        }

        public virtual void OnFetch()
        {
        }

        public virtual void OnRelease()
        {
        }
    }
}
