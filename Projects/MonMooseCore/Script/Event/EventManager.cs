using System;
using System.Collections.Generic;

namespace MonMooseCore
{
    public class EventManager : Singleton<EventManager>
    {
        private Dictionary<int, AbstractEventGroup> m_eventGroupMap = new Dictionary<int, AbstractEventGroup>();
        private Dictionary<int, EventBlockGroup> m_blockGroupMap = new Dictionary<int, EventBlockGroup>();
        private Dictionary<int, int> m_eventId2groupIdMap = new Dictionary<int, int>();

        protected override void OnUnInit()
        {
            m_eventGroupMap.Clear();
        }

        public void SetBlockGroup(int groupId, int eventId)
        {
            EventBlockGroup group = GetBlockGroupByGroupId(groupId);
            group.AddEventId(eventId);
            m_eventId2groupIdMap[eventId] = groupId;
        }
        public void SetBlockFlag(int groupId, bool flag)
        {
            EventBlockGroup group = GetBlockGroupByGroupId(groupId);
            group.isBlocked = flag;
        }

        public void RegisterListener(int eventId, Action action)
        {
            var eventGroup = GetEventGroup<EventGroup>(eventId, true);
            if (eventGroup != null)
            {
                eventGroup.Register(action);
            }
        }

        public void RegisterListener<T0>(int eventId, Action<T0> action)
        {
            var eventGroup = GetEventGroup<EventGroup<T0>>(eventId, true);
            if (eventGroup != null)
            {
                eventGroup.Register(action);
            }
        }

        public void RegisterListener<T0, T1>(int eventId, Action<T0, T1> action)
        {
            var eventGroup = GetEventGroup<EventGroup<T0, T1>>(eventId, true);
            if (eventGroup != null)
            {
                eventGroup.Register(action);
            }
        }

        public void RegisterListener<T0, T1, T2>(int eventId, Action<T0, T1, T2> action)
        {
            var eventGroup = GetEventGroup<EventGroup<T0, T1, T2>>(eventId, true);
            if (eventGroup != null)
            {
                eventGroup.Register(action);
            }
        }

        public void RegisterListener<T0, T1, T2, T3>(int eventId, Action<T0, T1, T2, T3> action)
        {
            var eventGroup = GetEventGroup<EventGroup<T0, T1, T2, T3>>(eventId, true);
            if (eventGroup != null)
            {
                eventGroup.Register(action);
            }
        }

        public void UnRegisterListener(int eventId, Action action)
        {
            var eventGroup = GetEventGroup<EventGroup>(eventId);
            if (eventGroup != null)
            {
                eventGroup.UnRegister(action);
            }
        }

        public void UnRegisterListener<T0>(int eventId, Action<T0> action)
        {
            var eventGroup = GetEventGroup<EventGroup<T0>>(eventId);
            if (eventGroup != null)
            {
                eventGroup.UnRegister(action);
            }
        }

        public void UnRegisterListener<T0, T1>(int eventId, Action<T0, T1> action)
        {
            var eventGroup = GetEventGroup<EventGroup<T0, T1>>(eventId);
            if (eventGroup != null)
            {
                eventGroup.UnRegister(action);
            }
        }

        public void UnRegisterListener<T0, T1, T2>(int eventId, Action<T0, T1, T2> action)
        {
            var eventGroup = GetEventGroup<EventGroup<T0, T1, T2>>(eventId);
            if (eventGroup != null)
            {
                eventGroup.UnRegister(action);
            }
        }

        public void UnRegisterListener<T0, T1, T2, T3>(int eventId, Action<T0, T1, T2, T3> action)
        {
            var eventGroup = GetEventGroup<EventGroup<T0, T1, T2, T3>>(eventId);
            if (eventGroup != null)
            {
                eventGroup.UnRegister(action);
            }
        }

        public void Broadcast(int eventId)
        {
            var eventGroup = GetEventGroup<EventGroup>(eventId);
            if (eventGroup != null)
            {
                EventBlockGroup group = GetBlockGroupByEventId(eventId);
                if (group != null && group.isBlocked)
                {
                    EventGroup.Params prms = ClassPoolManager.instance.Fetch<EventGroup.Params>(this);
                    prms.Set(eventGroup);
                    group.AddParams(prms);
                }
                else
                {
                    eventGroup.Broadcast();
                }
            }
        }

        public void Broadcast<T0>(int eventId, T0 arg0)
        {
            var eventGroup = GetEventGroup<EventGroup<T0>>(eventId);
            if (eventGroup != null)
            {
                EventBlockGroup group = GetBlockGroupByEventId(eventId);
                if (group != null && group.isBlocked)
                {
                    EventGroup<T0>.Params prms = ClassPoolManager.instance.Fetch<EventGroup<T0>.Params>(this);
                    prms.Set(eventGroup, arg0);
                    group.AddParams(prms);
                }
                else
                {
                    eventGroup.Broadcast(arg0);
                }
            }
        }

        public void Broadcast<T0, T1>(int eventId, T0 arg0, T1 arg1)
        {
            var eventGroup = GetEventGroup<EventGroup<T0, T1>>(eventId);
            if (eventGroup != null)
            {
                EventBlockGroup group = GetBlockGroupByEventId(eventId);
                if (group != null && group.isBlocked)
                {
                    EventGroup<T0, T1>.Params prms = ClassPoolManager.instance.Fetch<EventGroup<T0, T1>.Params>(this);
                    prms.Set(eventGroup, arg0, arg1);
                    group.AddParams(prms);
                }
                else
                {
                    eventGroup.Broadcast(arg0, arg1);
                }
            }
        }

        public void Broadcast<T0, T1, T2>(int eventId, T0 arg0, T1 arg1, T2 arg2)
        {
            var eventGroup = GetEventGroup<EventGroup<T0, T1, T2>>(eventId);
            if (eventGroup != null)
            {
                EventBlockGroup group = GetBlockGroupByEventId(eventId);
                if (group != null && group.isBlocked)
                {
                    EventGroup<T0, T1, T2>.Params prms = ClassPoolManager.instance.Fetch<EventGroup<T0, T1, T2>.Params>(this);
                    prms.Set(eventGroup, arg0, arg1, arg2);
                    group.AddParams(prms);
                }
                else
                {
                    eventGroup.Broadcast(arg0, arg1, arg2);
                }
            }
        }

        public void Broadcast<T0, T1, T2, T3>(int eventId, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            var eventGroup = GetEventGroup<EventGroup<T0, T1, T2, T3>>(eventId);
            if (eventGroup != null)
            {
                EventBlockGroup group = GetBlockGroupByEventId(eventId);
                if (group != null && group.isBlocked)
                {
                    EventGroup<T0, T1, T2, T3>.Params prms = ClassPoolManager.instance.Fetch<EventGroup<T0, T1, T2, T3>.Params>(this);
                    prms.Set(eventGroup, arg0, arg1, arg2, arg3);
                    group.AddParams(prms);
                }
                else
                {
                    eventGroup.Broadcast(arg0, arg1, arg2, arg3);
                }
            }
        }

        public string GetAllDelegateRefLog(Type enumType = null)
        {
            string str = string.Empty;
            Dictionary<int, AbstractEventGroup>.Enumerator enumerator = m_eventGroupMap.GetEnumerator();
            List<Type> cacheList = new List<Type>();
            while (enumerator.MoveNext())
            {
                int key = enumerator.Current.Key;
                AbstractEventGroup value = enumerator.Current.Value;
                if (value.count > 0)
                {
                    cacheList.Clear();
                    for (int i = 0; i < value.count; ++i)
                    {
                        Type type = value[i].Target.GetType();
                        if (!cacheList.Contains(type))
                        {
                            cacheList.Add(type);
                        }
                    }
                    str += enumType == null ? key.ToString() : Enum.GetName(enumType, key);
                    for (int i = 0; i < cacheList.Count; ++i)
                    {
                        str += ",";
                        str += cacheList[i].Name;
                    }
                }
                str += "\r\n";
            }
            enumerator.Dispose();
            return str;
        }

        private T GetEventGroup<T>(int eventId, bool autoCreate = false) where T : AbstractEventGroup, new()
        {
            if (eventId < 0)
            {
                return null;
            }
            AbstractEventGroup group;
            if (!m_eventGroupMap.TryGetValue(eventId, out group) && autoCreate)
            {
                group = new T();
                m_eventGroupMap.Add(eventId, group);
            }
            T ret = group as T;
            if (ret == null)
            {
                DebugUtility.LogError(string.Format("Error: {0} Has More Than One ParamGroups!!!", eventId));
                return null;
            }
            return ret;
        }

        private EventBlockGroup GetBlockGroupByGroupId(int groupId)
        {
            EventBlockGroup group;
            if (!m_blockGroupMap.TryGetValue(groupId, out group))
            {
                group = new EventBlockGroup();
                m_blockGroupMap.Add(groupId, group);
            }
            return group;
        }

        private EventBlockGroup GetBlockGroupByEventId(int eventId)
        {
            int groupId;
            if (m_eventId2groupIdMap.TryGetValue(eventId, out groupId))
            {
                return GetBlockGroupByGroupId(groupId);
            }
            return null;
        }
    }
}
