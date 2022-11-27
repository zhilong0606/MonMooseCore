namespace MonMoose.Core
{
    public interface IClassPoolObj
    {
        int checkPointId { get; set; }
        object causer { get; set; }
        ClassPool creator { get; set; }
        void OnFetch();
        void OnRelease();
    }
}