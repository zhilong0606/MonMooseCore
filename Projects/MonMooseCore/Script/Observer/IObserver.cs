namespace MonMoose.Core
{
    public interface IObserver
    {
        void OnReceive(int eventId, IObserveSubject subject);
    }
}
