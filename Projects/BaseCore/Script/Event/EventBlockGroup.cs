using System.Collections.Generic;

namespace MonMooseCore
{
    public class EventBlockGroup
    {
        private List<AbstractEventParams> m_paramsList = new List<AbstractEventParams>();
        private List<int> m_eventIdList = new List<int>();
        private bool m_isBlocked;

        public bool isBlocked
        {
            get { return m_isBlocked; }
            set
            {
                if (value != m_isBlocked)
                {
                    if (!value)
                    {
                        Broadcast();
                    }
                    m_isBlocked = value;
                }
                
            }
        }

        public void AddEventId(int eventId)
        {
            if (!m_eventIdList.Contains(eventId))
            {
                m_eventIdList.Add(eventId);
            }
        }

        public void AddParams(AbstractEventParams prms)
        {
            m_paramsList.Add(prms);
        }

        private void Broadcast()
        {
            for (int i = 0; i < m_paramsList.Count; ++i)
            {
                AbstractEventParams prms = m_paramsList[i];
                prms.Broadcast();
                prms.Release();
            }
            m_paramsList.Clear();
        }
    }
}
