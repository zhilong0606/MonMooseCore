using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace MonMoose.Core
{
    public class UIMedium
    {
        private UIMediumConfig m_config;
        private Dictionary<string, UIController> m_ctrlMap = new Dictionary<string, UIController>();
        private List<UIMediumResourceLoader> m_loaderList = new List<UIMediumResourceLoader>();
        private CommandMachine m_cmdMachine = new CommandMachine();
        private UIMediumContext m_curContext;
        private bool m_destroy;
        private Action<UIMedium> m_actionOnOpenEnd;
        private Action<UIMedium> m_actionOnCloseEnd;
        private UIMediumState m_mediumState;
        private float m_openedTime;
        private bool m_immediately;
        private bool m_initialized;

        private bool m_isStartingAsync;


        public UIMediumId mediumId
        {
            get { return m_config.mediumId; }
        }

        public UIMediumState mediumState
        {
            get { return m_mediumState; }
        }

        public UIMediumContext curContext
        {
            get { return m_curContext; }
        }

        public float openedTime
        {
            get { return m_openedTime; }
        }

        public bool stackOptimizable
        {
            get { return m_config.stackOptimizable; }
        }

        public void Init(UIMediumConfig config)
        {
            if (m_initialized)
            {
                return;
            }
            m_mediumState = UIMediumState.Closed;
            m_config = config;
            m_initialized = true;
            OnInit();
        }

        public void UnInit()
        {
            if (!m_initialized)
            {
                return;
            }
            OnUnInit();
            foreach (var kv in m_ctrlMap)
            {
                kv.Value.UnInit();
            }
            m_ctrlMap.Clear();
            OnClearControllers();
            m_initialized = false;
        }

        public void Open(UIMediumContext context, bool isAsync, bool immediately = false, Action<UIMedium> actionOnOpenEnd = null)
        {
            UIMediumCommandOpen openCmd = CreateCommand<UIMediumCommandOpen>();
            openCmd.context = context;
            openCmd.isAsync = isAsync;
            openCmd.immediately = immediately;
            openCmd.actionOnOpen = OpenInternal;
            openCmd.actionOnOpenEnd = actionOnOpenEnd;
            PushCommand(openCmd);
        }

        public void Close(bool immediately = false, bool destroy = false, Action<UIMedium> actionOnCloseEnd = null)
        {
            UIMediumCommandClose closeCmd = CreateCommand<UIMediumCommandClose>();
            closeCmd.immediately = immediately;
            closeCmd.destroy = destroy;
            closeCmd.actionOnClose = CloseInternal;
            closeCmd.actionOnCloseEnd = actionOnCloseEnd;
            PushCommand(closeCmd);
        }

        public bool HasTags(UIMediumTagId[] tags)
        {
            if (tags == null || tags.Length == 0 || m_config.tagIdList.Count == 0)
            {
                return false;
            }
            for (int i = 0; i < m_config.tagIdList.Count; ++i)
            {
                for (int j = 0; j < tags.Length; ++j)
                {
                    if (m_config.tagIdList[i] == tags[j])
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        protected void PlayOpen(bool immediately)
        {
            foreach (var kv in m_ctrlMap)
            {
                kv.Value.SetActiveSafely(true);
            }
            if (immediately)
            {
                PlayOpenEnd();
            }
            else
            {
                OnPlayOpen();
            }
        }

        protected virtual void OnPlayOpen()
        {
            PlayOpenEnd();
        }

        protected void PlayOpenEnd()
        {
            m_mediumState = UIMediumState.Opened;
            OnPlayOpenEnd();
            if (m_actionOnOpenEnd != null)
            {
                m_actionOnOpenEnd(this);
                m_actionOnOpenEnd = null;
            }
            m_openedTime = Time.realtimeSinceStartup;
            UIMediumManager.instance.RefreshStack();
        }

        protected void PlayClose(bool immediately)
        {
            if (immediately)
            {
                PlayCloseEnd();
            }
            else
            {
                OnPlayClose();
            }
        }

        protected virtual void OnPlayClose()
        {
            PlayCloseEnd();
        }

        protected void PlayCloseEnd()
        {
            m_mediumState = UIMediumState.Closed;
            List<UIController> tempList = new List<UIController>();
            foreach (var kv in m_ctrlMap)
            {
                tempList.Add(kv.Value);
            }
            foreach (var ctrl in tempList)
            {
                ctrl.SetActiveSafely(false);
                UIMultiLayerConfig multiLayerConfig = ctrl.GetComponent<UIMultiLayerConfig>();
                if (multiLayerConfig != null)
                {
                    SceneLayerManager.instance.PopLayerList(multiLayerConfig.layerList, false);
                }
                else
                {
                    SceneLayerBase layerBase = ctrl.GetComponent<SceneLayerBase>();
                    SceneLayerManager.instance.PopLayer(layerBase, false);
                }
                if (m_destroy)
                {
                    DestroyController(ctrl);
                }
            }
            OnPlayCloseEnd();
            if (m_actionOnCloseEnd != null)
            {
                m_actionOnCloseEnd(this);
                m_actionOnCloseEnd = null;
            }
        }


        protected UIMedium TransferOpen(UIMediumContext context, bool isAsync)
        {
            context.preContext = curContext;
            UIMedium medium = UIMediumManager.instance.Open(context, isAsync);
            return medium;
        }

        protected void ReturnToPreMedium()
        {
            UIMediumContext preContext = GetPreContext();
            if (preContext != null)
            {
                UIMediumManager.instance.Open(preContext, true);
            }
        }

        protected virtual void OnCollectViewLoader()
        {
            for (int i = 0; i < m_config.viewConfigList.Count; ++i)
            {
                UIViewConfig viewConfig = m_config.viewConfigList[i];
                if (!NeedLoadView(viewConfig))
                {
                    continue;
                }
                if (!m_ctrlMap.ContainsKey(viewConfig.nameStr))
                {
                    AddLoader<GameObject>(viewConfig.prefabWeakRef.path);
                }
            }
        }

        private void StartLoad(bool isAsync)
        {
            if (m_loaderList.Count > 0)
            {
                for (int i = 0; i < m_loaderList.Count; ++i)
                {
                    m_loaderList[i].StartLoad(isAsync, OnLoaderLoadEnd);
                }
            }
            else
            {
                OnLoadAllResourcesEnd();
            }
        }

        private void OnLoaderLoadEnd(string path)
        {
            for (int i = 0; i < m_loaderList.Count; ++i)
            {
                if (m_loaderList[i].path == path)
                {
                    m_loaderList[i].Release();
                    m_loaderList.RemoveAt(i);
                    break;
                }
            }
            if (m_loaderList.Count == 0)
            {
                OnLoadAllResourcesEnd();
            }
        }

        private void OnLoadAllResourcesEnd()
        {
            OnReady();
        }

        protected virtual void OnReady()
        {
            CreateControllers();
            OnRefreshControllers();
            PlayOpen(m_immediately);
        }

        #region Context
        private void InputContext(UIMediumContext context)
        {
            if (m_curContext == null)
            {
                m_curContext = context;
            }
            else
            {
                m_curContext.OverrideFrom(context);
            }
        }

        public UIMediumContext GetPreContext()
        {
            if (m_curContext != null)
            {
                return m_curContext.preContext;
            }
            return null;
        }
        #endregion

        #region Loader
        protected void AddLoader(UIMediumResourceLoader loader)
        {
            m_loaderList.Add(loader);
        }

        protected void AddLoader<T>(string path) where T : UnityEngine.Object
        {
            if (HasLoader(path))
            {
                return;
            }
            UIMediumResourceLoader<T> loader = ClassPoolManager.instance.Fetch<UIMediumResourceLoader<T>>();
            loader.Set(path);
            AddLoader(loader);
        }

        protected bool HasLoader(string path)
        {
            for (int i = 0; i < m_loaderList.Count; ++i)
            {
                if (m_loaderList[i].path == path)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        private void OpenInternal(UIMediumContext context, bool isAsync, bool immediately, Action<UIMedium> actionOnOpenEnd)
        {
            m_isStartingAsync = isAsync;
            m_mediumState = UIMediumState.Opening;
            m_immediately = immediately;
            m_actionOnOpenEnd = actionOnOpenEnd;
            InputContext(context);
            OnCollectViewLoader();
            OnCollectResourceLoader();
            StartLoad(isAsync);
        }

        private void CloseInternal(bool immediately, bool destroy, Action<UIMedium> actionOnCloseEnd)
        {
            m_immediately = immediately;
            m_destroy = destroy;
            m_actionOnCloseEnd = actionOnCloseEnd;
            if (m_mediumState == UIMediumState.Closed || m_mediumState == UIMediumState.Closing)
            {
                Debug.LogError(string.Format("[UIMedium] CloseInternal Cannot Close Not Opened:{0} state:{1}", mediumId, m_mediumState));
                PlayCloseEnd();
                return;
            }
            m_mediumState = UIMediumState.Closing;
            PlayClose(immediately);
        }

        private void CreateControllers()
        {
            for (int i = 0; i < m_config.viewConfigList.Count; ++i)
            {
                UIViewConfig viewConfig = m_config.viewConfigList[i];
                if (!NeedLoadView(viewConfig))
                {
                    continue;
                }
                CreateController(viewConfig.nameStr);
            }
        }

        public T CreateCommand<T>() where T : UIMediumCommand
        {
            T cmd = ClassPoolManager.instance.Fetch<T>();
            cmd.Init(this);
            return cmd;
        }

        public void PushCommand(Command cmd)
        {
            m_cmdMachine.PushCommand(cmd, null);
        }

        public void Tick(TimeSlice timeSlice)
        {
            OnTick(timeSlice);
        }

        protected UIController CreateController(string name)
        {
            if (m_ctrlMap.ContainsKey(name))
            {
                return m_ctrlMap[name];
            }
            UIViewConfig viewConfig = m_config.GetViewConfigByName(name);
            if (viewConfig == null)
            {
                return null;
            }
            string path = viewConfig.prefabWeakRef.path;
            GameObject prefab = ResourceManager.instance.LoadSync<GameObject>(path);
            if (prefab == null)
            {
                return null;
            }
            GameObject root = GameObject.Instantiate(prefab);
            UIMultiLayerConfig multiLayerConfig = root.GetComponent<UIMultiLayerConfig>();
            if (multiLayerConfig != null)
            {
                SceneLayerManager.instance.PushLayerList(multiLayerConfig.layerList, !m_isStartingAsync);
            }
            else
            {
                SceneLayerBase layerBase = root.GetComponent<SceneLayerBase>();
                SceneLayerManager.instance.PushLayer(layerBase, !m_isStartingAsync);
            }
            UIController ctrl = root.AddComponent(viewConfig.classType) as UIController;
            ctrl.Init(null);
            m_ctrlMap.Add(viewConfig.nameStr, ctrl);
            OnAddController(viewConfig.nameStr, ctrl);
            return ctrl;
        }

        protected void DestroyController(UIController ctrl)
        {
            if (m_ctrlMap.ContainsValue(ctrl))
            {
                m_ctrlMap.RemoveValue(ctrl);
                OnRemoveController(ctrl);
                ctrl.UnInit();
                GameObject.Destroy(ctrl.gameObject);
            }
        }

        protected virtual void OnInit() { }
        protected virtual void OnUnInit() { }
        protected virtual void OnRefreshControllers() { }
        protected virtual void OnCollectResourceLoader() { }
        protected virtual void OnClearControllers() { }
        protected virtual void OnPlayOpenEnd() { }
        protected virtual void OnPlayCloseEnd() { }
        protected virtual void OnTick(TimeSlice timeSlice) { }
        protected virtual void OnAddController(string name, UIController ctrl) { }
        protected virtual void OnRemoveController(UIController ctrl) { }
        protected virtual bool NeedLoadView(UIViewConfig config) { return true; }
    }
}
