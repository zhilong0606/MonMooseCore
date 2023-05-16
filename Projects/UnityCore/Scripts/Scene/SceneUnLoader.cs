using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MonMoose.Core
{
    public class SceneUnLoader : ClassPoolObj
    {
        private SceneInfo m_sceneInfo;
        private AsyncOperation m_asyncOperation;
        private ProcessId m_processId;
        private Action<SceneUnLoader> m_actionOnUnLoadEnd;

        public SceneInfo sceneInfo
        {
            get { return m_sceneInfo; }
        }

        public override void OnRelease()
        {
            m_sceneInfo = null;
            m_asyncOperation = null;
            m_processId = ProcessId.None;
            m_actionOnUnLoadEnd = null;
            base.OnRelease();
        }

        public void StartUnLoad(SceneInfo sceneInfo, Action<SceneUnLoader> actionOnUnLoadEnd)
        {
            m_sceneInfo = sceneInfo;
            m_processId = ProcessId.Start;
            m_actionOnUnLoadEnd = actionOnUnLoadEnd;
            if (sceneInfo == null)
            {
                UnLoadEnd(false);
            }
            else if (m_sceneInfo.scene.IsValid())
            {
                if (UnityEngine.SceneManagement.SceneManager.GetActiveScene() == m_sceneInfo.scene)
                {
                    UnLoadEnd(false);
                }
            }
            else
            {
                UnLoadEnd(false);
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
                    //#if !UNITY_EDITOR
                    if (m_sceneInfo.useIndex)
                    {
                        m_asyncOperation = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(m_sceneInfo.index);
                    }
                    else
                    {
                        m_asyncOperation = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(m_sceneInfo.path);
                    }
                    //#else
                    //                    m_asyncOperation = UnityEditor.SceneManagement.EditorSceneManager.UnloadSceneAsync(m_sceneInfo.scene);
                    //#endif
                    if (m_asyncOperation == null)
                    {
                        Debug.LogError(string.Format("[SceneInfo] UnLoad fail:{0}, AsyncOperation is null", m_sceneInfo.path));
                        UnLoadEnd(false);
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
                        m_processId = ProcessId.End;
                        UnLoadEnd(true);
                    }
                    break;
            }
        }


        private void UnLoadEnd(bool success)
        {
            NotifyLoadEnd();
        }

        private void NotifyLoadEnd()
        {
            if (m_actionOnUnLoadEnd != null)
            {
                m_actionOnUnLoadEnd(this);
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
