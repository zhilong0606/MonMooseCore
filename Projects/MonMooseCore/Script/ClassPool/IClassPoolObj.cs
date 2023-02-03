namespace MonMoose.Core
{
    public interface IClassPoolObj
    {
        int checkPointId { get; set; }
        object causer { get; set; }
        ClassPool creator { get; set; }
        void OnFetch();
        void OnRelease();

        System.Diagnostics.StackTrace stackTrace { get; set; }
        string createLog { get; set; }
    }
}