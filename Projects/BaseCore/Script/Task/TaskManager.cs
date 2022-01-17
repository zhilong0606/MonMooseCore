using System;
using System.Collections.Generic;

namespace MonMooseCore
{
    public class TaskManager : Singleton<TaskManager>
    {
        private List<Task> m_taskList = new List<Task>();
        private Action<Task> m_actionOnRemove;

        protected override void OnInit()
        {
            base.OnInit();
            TickManager.instance.RegisterGlobalTick(OnTick);
        }

        public void AddTask(Task task)
        {
            m_taskList.Add(task);
        }

        private void OnTick(float deltaTime)
        {
            for (int i = 0; i < m_taskList.Count; ++i)
            {
                Task task = m_taskList[i];
                task.Tick(deltaTime);
            }
            for (int i = m_taskList.Count - 1; i >= 0; --i)
            {
                Task task = m_taskList[i];
                if (task.state == Task.EState.End)
                {
                    task.Remove();
                    task.Release();
                    m_taskList.RemoveAt(i);
                }
            }
        }
    }
}
