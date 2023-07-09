namespace MonMoose.Core
{
    public abstract class ClassPoolObj : IClassPoolObj
    {
        public ClassPool creator { get; set; }
        public int checkPointId { get; set; }
        public object causer { get; set; }
        public System.Diagnostics.StackTrace fetchStackTrace { get; set; }
        public System.Diagnostics.StackTrace releaseStackTrace { get; set; }
        public string fetchStackTraceLog { get; set; }
        public string releaseStackTraceLog { get; set; }

        public virtual void OnFetch()
        {
        }

        public virtual void OnRelease()
        {
        }
    }
}
