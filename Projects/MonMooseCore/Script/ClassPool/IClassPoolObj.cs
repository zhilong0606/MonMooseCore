namespace MonMoose.Core
{
    public interface IClassPoolObj
    {
        int checkPointId { get; set; }
        object causer { get; set; }
        ClassPool creator { get; set; }
        void OnFetch();
        void OnRelease();

        System.Diagnostics.StackTrace fetchStackTrace { get; set; }
        System.Diagnostics.StackTrace releaseStackTrace { get; set; }
        string fetchStackTraceLog { get; set; }
        string releaseStackTraceLog { get; set; }
    }
}