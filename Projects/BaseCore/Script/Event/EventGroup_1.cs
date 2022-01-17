using System;

namespace MonMooseCore
{
    public class EventGroup<T0> : AbstractEventGroup
    {
        public bool Contains(Action<T0> d)
        {
            return Contains(d as Delegate);
        }

        public void Register(Action<T0> d)
        {
            Register(d as Delegate);
        }

        public void UnRegister(Action<T0> d)
        {
            UnRegister(d as Delegate);
        }

        public void Broadcast(T0 arg0)
        {
            bool findNotMatch = false;
            Action<T0> action;
            for (int i = 0; i < m_delegateList.Count; ++i)
            {
                action = m_delegateList[i] as Action<T0>;
                if (action != null)
                {
                    try
                    {
                        action(arg0);
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
            public EventGroup<T0> m_eventGroup;
            public T0 value0;

            public void Set(EventGroup<T0> eventGroup, T0 v0)
            {
                m_eventGroup = eventGroup;
                value0 = v0;
            }

            public override void OnRelease()
            {
                m_eventGroup = null;
                value0 = default(T0);
            }

            public override void Broadcast()
            {
                m_eventGroup.Broadcast(value0);
            }
        }
    }
}