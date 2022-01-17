using System;
using System.Collections.Generic;

namespace MonMooseCore
{
    public class TickManager : Singleton<TickManager>
    {
        private Dictionary<int, List<Action<float>>> m_globalTickListMap = new Dictionary<int, List<Action<float>>>();
        private Dictionary<int, TickProcess> m_tickProcessMap = new Dictionary<int, TickProcess>();
        private List<int> m_globalIdList = new List<int>();

        public void RegisterGlobalTick(Action<float> action)
        {
            RegisterGlobalTick(0, action);
        }

        public void UnRegisterGlobalTick(Action<float> action)
        {
            UnRegisterGlobalTick(0, action);
        }

        public void RegisterGlobalTick(int id, Action<float> action)
        {
            List<Action<float>> list;
            if (!m_globalTickListMap.TryGetValue(id, out list))
            {
                list = new List<Action<float>>();
                m_globalTickListMap.Add(id, list);
                m_globalIdList.Add(id);
                m_globalIdList.Sort();
            }
            if (!list.Contains(action))
            {
                list.Add(action);
            }
        }

        public void UnRegisterGlobalTick(int id, Action<float> action)
        {
            List<Action<float>> list;
            if (m_globalTickListMap.TryGetValue(id, out list))
            {
                list.Remove(action);
            }
        }

        public void CreateTickProcess(int id, float interval)
        {
            TickProcess process = GetTickProcess(id);
            if (process != null)
            {
                DebugUtility.LogError("Error: Trying Create TickProcess has same Id");
            }
            process = new TickProcess();
            process.Init(interval);
            m_tickProcessMap.Add(id, process);
        }

        public void RegisterTickProcess(int id, Action<TickProcess> action)
        {
            TickProcess process = GetTickProcess(id);
            if (process != null)
            {
                process.RegisterListener(action);
            }
        }

        public void UnRegisterTickProcess(int id, Action<TickProcess> action)
        {
            TickProcess process = GetTickProcess(id);
            if (process != null)
            {
                process.UnRegisterListener(action);
            }
        }

        public void StartTickProcess(int id)
        {
            TickProcess process = GetTickProcess(id);
            if (process != null)
            {
                process.Start();
            }
        }

        public void StopTickProcess(int id)
        {
            TickProcess process = GetTickProcess(id);
            if (process != null)
            {
                process.Stop();
            }
        }

        public void Tick(float deltaTime)
        {
            TickGlobal(deltaTime, false);
            Dictionary<int, TickProcess>.Enumerator tickIter = m_tickProcessMap.GetEnumerator();
            while (tickIter.MoveNext())
            {
                tickIter.Current.Value.Tick(deltaTime);
            }
            tickIter.Dispose();
            TickGlobal(deltaTime, true);
        }

        private void TickGlobal(float deltaTime, bool tickPositive)
        {
            for (int i = 0; i < m_globalIdList.Count; ++i)
            {
                int id = m_globalIdList[i];
                if (tickPositive != id > 0)
                {
                    continue;
                }
                List<Action<float>> list = m_globalTickListMap.GetClassValue(id);
                if (list == null)
                {
                    continue;
                }
                for (int j = 0; j < list.Count; ++j)
                {
                    Action<float> action = list[j];
                    if (action != null)
                    {
                        action(deltaTime);
                    }
                }
            }
        }

        private TickProcess GetTickProcess(int id)
        {
            return m_tickProcessMap.GetClassValue(id);
        }

        private TickProcess GetTickProcess(Action<TickProcess> action)
        {
            TickProcess process;
            Dictionary<int, TickProcess>.Enumerator enumerator = m_tickProcessMap.GetEnumerator();
            while (enumerator.MoveNext())
            {
                process = enumerator.Current.Value;
                if (process.ContainsAction(action))
                {
                    return process;
                }
            }
            enumerator.Dispose();
            return null;
        }
    }
}