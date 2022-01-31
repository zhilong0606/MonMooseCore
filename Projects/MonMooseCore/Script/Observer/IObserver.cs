namespace MonMooseCore
{
    public interface IObserver
    {
        void OnReceive(int eventId, IObserveSubject subject);
    }
}
