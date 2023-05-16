using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MonMoose.Core
{
    public class SceneManager : Singleton<SceneManager>
    {
        private SceneInfo m_curSceneInfo;
        private List<SceneInfo> m_addSceneInfoList = new List<SceneInfo>();
        private List<SceneLoader> m_loaderList = new List<SceneLoader>();
        private List<SceneUnLoader> m_unLoaderList = new List<SceneUnLoader>();
        private static readonly List<SceneLoader> m_tempLoaderList = new List<SceneLoader>();
        private static readonly List<SceneUnLoader> m_tempUnLoaderList = new List<SceneUnLoader>();
        private Command m_curCmd;
        private List<Command> m_cmdList = new List<Command>();

        protected override void OnInit()
        {
            base.OnInit();
            TickManager.instance.RegisterGlobalTick(OnTick);
        }

        protected override void OnUnInit()
        {
            base.OnUnInit();
            TickManager.instance.UnRegisterGlobalTick(OnTick);
        }

        public void ChangeScene(SceneBuildId id, Action<SceneInfo> actionOnEnd = null)
        {
            SceneInfo sceneInfo = new SceneInfo((int)id);
            ChangeScene(sceneInfo, actionOnEnd);
        }

        public void ChangeScene(string path, Action<SceneInfo> actionOnEnd = null)
        {
            SceneInfo sceneInfo = new SceneInfo(path);
            ChangeScene(sceneInfo, actionOnEnd);
        }

        public void ChangeScene(SceneInfo sceneInfo, Action<SceneInfo> actionOnEnd = null)
        {
            if (m_curSceneInfo != null)
            {
                PushUnLoadCmd(m_curSceneInfo, null);
            }
            PushLoadCmd(sceneInfo, null);
            PushSetActiveCmd(sceneInfo, actionOnEnd);
        }

        public void SetSceneActive(SceneInfo sceneInfo)
        {
            if (sceneInfo != null && sceneInfo.scene.IsValid())
            {
                UnityEngine.SceneManagement.SceneManager.SetActiveScene(sceneInfo.scene);
            }
        }

        public void ChangeEmptyScene(Action<SceneInfo> actionOnEnd = null)
        {
            ChangeScene(SceneBuildId.Empty, actionOnEnd);
        }

        private void PushSetActiveCmd(SceneInfo sceneInfo, Action<SceneInfo> actionOnEnd)
        {
            PushCmd(CommandType.SetActive, sceneInfo, actionOnEnd);
        }

        private void PushLoadCmd(SceneInfo sceneInfo, Action<SceneInfo> actionOnEnd)
        {
            PushCmd(CommandType.Load, sceneInfo, actionOnEnd);
        }

        private void PushUnLoadCmd(SceneInfo sceneInfo, Action<SceneInfo> actionOnEnd)
        {
            PushCmd(CommandType.UnLoad, sceneInfo, actionOnEnd);
        }

        private void PushCmd(CommandType cmdType, SceneInfo sceneInfo, Action<SceneInfo> actionOnEnd)
        {
            Command cmd = ClassPoolManager.instance.Fetch<Command>();
            cmd.cmdType = cmdType;
            cmd.sceneInfo = sceneInfo;
            cmd.actionOnEnd = actionOnEnd;
            m_cmdList.Add(cmd);

            TryExecuteNextCmd();
        }

        private void TryExecuteNextCmd()
        {
            if (m_curCmd != null)
            {
                return;
            }
            if (m_cmdList.Count == 0)
            {
                return;
            }
            ExecuteNextCmd();
        }

        private void FinishCurCmdAndTryExecuteNextCmd()
        {
            if (m_curCmd != null)
            {
                m_curCmd.actionOnEnd.InvokeSafely(m_curCmd.sceneInfo);
                m_curCmd.Release();
                m_curCmd = null;
            }
            TryExecuteNextCmd();
        }

        private void ExecuteNextCmd()
        {
            if (m_cmdList.Count == 0)
            {
                return;
            }
            m_curCmd = m_cmdList[0];
            m_cmdList.RemoveAt(0);
            ExecuteCmd(m_curCmd);
        }

        private void ExecuteCmd(Command cmd)
        {
            switch (cmd.cmdType)
            {
                case CommandType.Load:
                    ExecuteLoadCmd(cmd);
                    break;
                case CommandType.UnLoad:
                    ExecuteUnLoadCmd(cmd);
                    break;
                case CommandType.SetActive:
                    ExecuteSetActiveCmd(cmd);
                    break;
            }
        }

        private void ExecuteLoadCmd(Command cmd)
        {
            SceneLoader loader = CreateLoader();
            loader.StartLoad(cmd.sceneInfo, OnLoaderLoadEnd);
        }

        private void ExecuteUnLoadCmd(Command cmd)
        {
            SceneUnLoader unLoader = CreateUnLoader();
            unLoader.StartUnLoad(cmd.sceneInfo, OnUnLoaderUnLoadEnd);
        }

        private void ExecuteSetActiveCmd(Command cmd)
        {
            SetSceneActive(cmd.sceneInfo);
            m_curSceneInfo = cmd.sceneInfo;
            FinishCurCmdAndTryExecuteNextCmd();
        }

        private SceneLoader CreateLoader()
        {
            SceneLoader loader = ClassPoolManager.instance.Fetch<SceneLoader>();
            m_loaderList.Add(loader);
            return loader;
        }

        private SceneUnLoader CreateUnLoader()
        {
            SceneUnLoader unLoader = ClassPoolManager.instance.Fetch<SceneUnLoader>();
            m_unLoaderList.Add(unLoader);
            return unLoader;
        }

        private void OnLoaderLoadEnd(SceneLoader loader)
        {
            m_loaderList.Remove(loader);
            loader.Release();
            FinishCurCmdAndTryExecuteNextCmd();
        }

        private void OnUnLoaderUnLoadEnd(SceneUnLoader unLoader)
        {
            m_unLoaderList.Remove(unLoader);
            unLoader.Release();
            if (m_curSceneInfo == unLoader.sceneInfo)
            {
                m_curSceneInfo = null;
            }
            FinishCurCmdAndTryExecuteNextCmd();
        }

        private void OnTick(TimeSlice timeSlice)
        {
            m_tempLoaderList.AddRange(m_loaderList);
            for (int i = 0; i < m_tempLoaderList.Count; ++i)
            {
                m_tempLoaderList[i].Tick();
            }
            m_tempLoaderList.Clear();

            m_tempUnLoaderList.AddRange(m_unLoaderList);
            for (int i = 0; i < m_tempUnLoaderList.Count; ++i)
            {
                m_tempUnLoaderList[i].Tick();
            }
            m_tempUnLoaderList.Clear();
        }

        private class Command : ClassPoolObj
        {
            public CommandType cmdType;
            public SceneInfo sceneInfo;
            public Action<SceneInfo> actionOnEnd;

            public override void OnRelease()
            {
                sceneInfo = null;
                actionOnEnd = null;
                base.OnRelease();
            }
        }

        private enum CommandType
        {
            Load,
            UnLoad,
            SetActive,
        }
    }
}
