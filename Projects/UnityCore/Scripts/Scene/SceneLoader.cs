using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MonMoose.Core
{
    public class SceneLoader : ClassPoolObj
    {
        private SceneInfo m_sceneInfo;
        private AsyncOperation m_asyncOperation;
        private ProcessId m_processId;
        private Action<SceneLoader> m_actionOnLoadEnd;

        public SceneInfo sceneInfo
        {
            get { return m_sceneInfo; }
        }

        public override void OnRelease()
        {
            m_sceneInfo = null;
            m_asyncOperation = null;
            m_processId = ProcessId.None;
            m_actionOnLoadEnd = null;
            base.OnRelease();
        }

        public void StartLoad(SceneInfo sceneInfo, Action<SceneLoader> actionOnLoadEnd)
        {
            m_sceneInfo = sceneInfo;
            m_processId = ProcessId.Start;
            m_actionOnLoadEnd = actionOnLoadEnd;
            if (sceneInfo == null)
            {
                LoadEnd(false);
            }
        }

        public void Tick()
        {
            switch (m_processId)
            {
                case ProcessId.Start:
                    m_processId = ProcessId.InitAsyncOperation;
                    Tick();
                    break;
                case ProcessId.InitAsyncOperation:
                    if (m_sceneInfo.useIndex)
                    {
                        m_asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(m_sceneInfo.index, new LoadSceneParameters(LoadSceneMode.Single, LocalPhysicsMode.None));
                    }
                    else
                    {
#if !UNITY_EDITOR
                        m_asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(m_sceneInfo.path, new LoadSceneParameters(LoadSceneMode.Single, LocalPhysicsMode.None));
#else
                        m_asyncOperation = UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(m_sceneInfo.path, new LoadSceneParameters(LoadSceneMode.Single, LocalPhysicsMode.None));
#endif
                    }
                    if (m_asyncOperation == null)
                    {
                        Debug.LogError(string.Format("[SceneInfo] Load fail:{0}, AsyncOperation is null", m_sceneInfo.path));
                        LoadEnd(false);
                    }
                    else
                    {
                        m_processId = ProcessId.WaitAsyncOperationDone;
                        Tick();
                    }
                    break;
                case ProcessId.WaitAsyncOperationDone:
                    if (m_asyncOperation.isDone)
                    {
                        m_processId = ProcessId.InitScene;
                        Tick();
                    }
                    break;
                case ProcessId.InitScene:
                    if (m_sceneInfo.useIndex)
                    {
                        m_sceneInfo.scene = UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex(m_sceneInfo.index);
                    }
                    else
                    {
                        string sceneName = Path.GetFileNameWithoutExtension(m_sceneInfo.path);
                        m_sceneInfo.scene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName);
                    }
                    if (m_sceneInfo.scene.IsValid())
                    {
                        m_processId = ProcessId.WaitSceneLoaded;
                        Tick();
                    }
                    else
                    {
                        Debug.LogError(string.Format("[SceneInfo] Load fail:{0}, Scene is invalid", m_sceneInfo.useIndex ? m_sceneInfo.index.ToString() : m_sceneInfo.path));
                        LoadEnd(false);
                    }
                    break;
                case ProcessId.WaitSceneLoaded:
                    if (m_sceneInfo.scene.isLoaded)
                    {
                        m_processId = ProcessId.End;
                        LoadEnd(true);
                    }
                    break;
            }
        }


        private void LoadEnd(bool success)
        {
            NotifyLoadEnd();
        }

        private void NotifyLoadEnd()
        {
            if (m_actionOnLoadEnd != null)
            {
                m_actionOnLoadEnd(this);
            }
        }

        private enum ProcessId
        {
            None,
            Start,
            InitAsyncOperation,
            WaitAsyncOperationDone,
            InitScene,
            WaitSceneLoaded,
            End,
        }
    }
}
