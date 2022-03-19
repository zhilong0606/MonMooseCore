namespace MonMoose.Core
{
    public interface IClassPoolObj
    {
        object causer { get; set; }
        ClassPool creator { get; set; }
        void OnFetch();
        void OnRelease();
    }
}