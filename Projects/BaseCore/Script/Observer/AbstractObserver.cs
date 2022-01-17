namespace MonMooseCore
{
    public abstract class AbstractObserver : IObserver
    {
        public void AddSubject(IObserveSubject subject)
        {
            subject.AddObserver(this);
        }

        public void RemoveSubject(IObserveSubject subject)
        {
            subject.RemoveObserver(this);
        }

        public abstract void OnReceive(int eventId, IObserveSubject subject);
    }
}
