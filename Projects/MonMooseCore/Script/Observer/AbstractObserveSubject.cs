using System.Collections.Generic;

namespace MonMoose.Core
{
    public abstract class AbstractObserveSubject : IObserveSubject
    {
        private List<IObserver> m_observerList = new List<IObserver>();

        public void AddObserver(IObserver observer)
        {
            if (!m_observerList.Contains(observer))
            {
                m_observerList.Add(observer);
            }
        }

        public void RemoveObserver(IObserver observer)
        {
            m_observerList.Remove(observer);
        }

        public void RemoveAllObservers()
        {
            m_observerList.Clear();
        }

        public void Notify(int eventId)
        {
            for (int i = 0; i < m_observerList.Count; ++i)
            {
                m_observerList[i].OnReceive(eventId, this);
            }
        }
    }
}
