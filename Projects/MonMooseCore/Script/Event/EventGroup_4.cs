using System;

namespace MonMooseCore
{
    public class EventGroup<T0, T1, T2, T3> : AbstractEventGroup
    {
        public bool Contains(Action<T0, T1, T2, T3> d)
        {
            return Contains(d as Delegate);
        }

        public void Register(Action<T0, T1, T2, T3> d)
        {
            Register(d as Delegate);
        }

        public void UnRegister(Action<T0, T1, T2, T3> d)
        {
            UnRegister(d as Delegate);
        }

        public void Broadcast(T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            bool findNotMatch = false;
            Action<T0, T1, T2, T3> action;
            for (int i = 0; i < m_delegateList.Count; ++i)
            {
                action = m_delegateList[i] as Action<T0, T1, T2, T3>;
                if (action != null)
                {
                    try
                    {
                        action(arg0, arg1, arg2, arg3);
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
            public EventGroup<T0, T1, T2, T3> m_eventGroup;
            public T0 value0;
            public T1 value1;
            public T2 value2;
            public T3 value3;

            public void Set(EventGroup<T0, T1, T2, T3> eventGroup, T0 v0, T1 v1, T2 v2, T3 v3)
            {
                m_eventGroup = eventGroup;
                value0 = v0;
                value1 = v1;
                value2 = v2;
                value3 = v3;
            }

            public override void OnRelease()
            {
                m_eventGroup = null;
                value0 = default(T0);
                value1 = default(T1);
                value2 = default(T2);
                value3 = default(T3);
            }

            public override void Broadcast()
            {
                m_eventGroup.Broadcast(value0, value1, value2, value3);
            }
        }
    }
}