using System;

namespace MonMoose.Core
{
    public class EventGroup<T0, T1> : AbstractEventGroup
    {
        public bool Contains(Action<T0, T1> d)
        {
            return Contains(d as Delegate);
        }

        public void Register(Action<T0, T1> d)
        {
            Register(d as Delegate);
        }

        public void UnRegister(Action<T0, T1> d)
        {
            UnRegister(d as Delegate);
        }

        public void Broadcast(T0 arg0, T1 arg1)
        {
            bool findNotMatch = false;
            Action<T0, T1> action;
            for (int i = 0; i < m_delegateList.Count; ++i)
            {
                action = m_delegateList[i] as Action<T0, T1>;
                if (action != null)
                {
                    try
                    {
                        action(arg0, arg1);
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
            public EventGroup<T0, T1> m_eventGroup;
            public T0 value0;
            public T1 value1;

            public void Set(EventGroup<T0, T1> eventGroup, T0 v0, T1 v1)
            {
                m_eventGroup = eventGroup;
                value0 = v0;
                value1 = v1;
            }

            public override void OnRelease()
            {
                m_eventGroup = null;
                value0 = default(T0);
                value1 = default(T1);
            }

            public override void Broadcast()
            {
                m_eventGroup.Broadcast(value0, value1);
            }
        }
    }
}