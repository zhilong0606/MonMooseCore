using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonMoose.Core
{
    public abstract class ResourceLoader
    {
        private ResourceCache m_cache = new ResourceCache();
        private List<ResourceLoadTask> m_taskList = new List<ResourceLoadTask>();
        private List<ResourceLoadTask> m_removingTaskList = new List<ResourceLoadTask>();
        private const char m_resourceSubNameSeperateChar = '$';

        public virtual bool isInitialized
        {
            get { return true; }
        }

        public void Init()
        {
            OnInit();
        }

        public void ClearAll()
        {
            m_cache.Clear();
        }

        protected void StartTask(ResourceLoadTask task)
        {
            for (int i = 0; i < m_taskList.Count; ++i)
            {
                if (task.fullPath == m_taskList[i].fullPath)
                {
                    task.Release();
                    return;
                }
            }
            m_taskList.Add(task);
            task.StartLoad();
        }

        public void Tick(TimeSlice timeSlice)
        {
            if (!isInitialized)
            {
                return;
            }
            int count = m_taskList.Count;
            for (int i = 0; i < count; ++i)
            {
                ResourceLoadTask task = m_taskList[i];
                task.Tick(timeSlice);
                if (task.isLoadEnd)
                {
                    m_removingTaskList.Add(task);
                }
            }
            count = m_removingTaskList.Count;
            if (count > 0)
            {
                for (int i = 0; i < count; ++i)
                {
                    RemoveTask(m_removingTaskList[i]);
                }
                m_removingTaskList.Clear();
            }
        }

        public T LoadSync<T>(string resourceFullPath) where T : UnityEngine.Object
        {
            if (!isInitialized)
            {
                return null;
            }
            UnityEngine.Object resourceObj;
            if (m_cache.TryGet(resourceFullPath, out resourceObj))
            {
                return resourceObj as T;
            }
            string resourcePath;
            string resourceSubName;
            AnalyzePath(resourceFullPath, out resourcePath, out resourceSubName);
            resourceObj = OnLoadSync<T>(resourcePath, resourceSubName);
            m_cache.Add(resourceFullPath, resourceObj);
            ResourceLoadTask runningTask;
            if (TryGetRunningTask(resourceFullPath, out runningTask))
            {
                runningTask.NotifyLoadObjectEnd();
                RemoveTask(runningTask);
            }
            return resourceObj as T;
        }

        public void LoadAsync<T>(string resourceFullPath, Action<string, UnityEngine.Object> actionOnLoadEnd) where T : UnityEngine.Object
        {
            ResourceLoadTask runningLoadTask;
            UnityEngine.Object resourceObj;
            if (m_cache.TryGet(resourceFullPath, out resourceObj))
            {
                ResourceLoadTaskAlreadyLoad loadTask = ClassPoolManager.instance.Fetch<ResourceLoadTaskAlreadyLoad>();
                loadTask.fullPath = resourceFullPath;
                loadTask.resourceObj = resourceObj;
                loadTask.AddActionOnLoadObjectEnd(actionOnLoadEnd);
                loadTask.actionOnLoadEnd = OnTaskLoadEnd;
                StartTask(loadTask);
            }
            else if (TryGetRunningTask(resourceFullPath, out runningLoadTask))
            {
                runningLoadTask.AddActionOnLoadObjectEnd(actionOnLoadEnd);
            }
            else
            {
                string resourceLoadPath;
                string resourceSubName;
                AnalyzePath(resourceFullPath, out resourceLoadPath, out resourceSubName);
                ResourceLoadTask loadTask = CreateLoadTask<T>();
                loadTask.fullPath = resourceFullPath;
                loadTask.loadPath = resourceLoadPath;
                loadTask.subName = resourceSubName;
                loadTask.AddActionOnLoadObjectEnd(actionOnLoadEnd);
                loadTask.actionOnLoadEnd = OnTaskLoadEnd;
                StartTask(loadTask);
            }
        }

        private void OnTaskLoadEnd(ResourceLoadTask task)
        {
            m_cache.Add(task.fullPath, task.resourceObj);
        }

        private void AnalyzePath(string path, out string resourcePath, out string resourceSubName)
        {
            resourcePath = path;
            resourceSubName = string.Empty;
            if (path.Contains(m_resourceSubNameSeperateChar))
            {
                string[] splits = path.Split(m_resourceSubNameSeperateChar);
                if (splits.Length == 2)
                {
                    resourcePath = splits[0];
                    resourceSubName = splits[1];
                }
            }
        }

        private bool TryGetRunningTask(string fullPath, out ResourceLoadTask task)
        {
            
            for (int i = 0; i < m_taskList.Count; ++i)
            {
                if (fullPath == m_taskList[i].fullPath)
                {
                    task = m_taskList[i];
                    return true;
                }
            }
            task = null;
            return false;
        }

        private void RemoveTask(ResourceLoadTask task)
        {
            task.Release();
            m_taskList.Remove(task);
        }

        protected virtual void OnInit() { }
        protected abstract T OnLoadSync<T>(string resourceLoadPath, string resourceSubName) where T : UnityEngine.Object;
        protected abstract ResourceLoadTask CreateLoadTask<T>() where T : UnityEngine.Object;
    }
}
