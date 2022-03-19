namespace MonMoose.Core
{
    public interface IObserveSubject
    {
        void AddObserver(IObserver observer);
        void RemoveObserver(IObserver observer);
        void Notify(int eventId);
    }
}
