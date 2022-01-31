using System;

namespace MonMooseCore
{
    public class EventGroup : AbstractEventGroup
    {
        public bool Contains(Action d)
        {
            return Contains(d as Delegate);
        }

        public void Register(Action d)
        {
            Register(d as Delegate);
        }

        public void UnRegister(Action d)
        {
            UnRegister(d as Delegate);
        }

        public void Broadcast()
        {
            bool findNotMatch = false;
            Action action;
            for (int i = 0; i < m_delegateList.Count; ++i)
            {
                action = m_delegateList[i] as Action;
                if (action != null)
                {
                    try
                    {
                        action();
                    }
                    catch (Exception e)
                    {
                        HandleException(e);
                    }
                }
                else if (!findNotMatch)
                {
                    findNotMatch = true;
                }
            }
            if (findNotMatch)
            {
                LogErrorNotMatchBroadcast();
            }
        }

        public class Params : AbstractEventParams
        {
            public EventGroup m_eventGroup;

            public void Set(EventGroup eventGroup)
            {
                m_eventGroup = eventGroup;
            }

            public override void OnRelease()
            {
                m_eventGroup = null;
            }

            public override void Broadcast()
            {
                m_eventGroup.Broadcast();
            }
        }
    }
}