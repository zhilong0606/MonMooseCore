using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace MonMoose.Core
{
    public class AnimatorPlayController : MonoBehaviour
    {
        private Animator m_animator; 
        private AnimatorOverrideController m_animOverCtrl;
        private List<Layer> m_layerList = new List<Layer>();
        private Dictionary<string, AnimationClip> m_clipMap = new Dictionary<string, AnimationClip>();
        private List<string> m_overrideKeyList = new List<string>();
        private List<AnimatorPlayCommand> m_commandList = new List<AnimatorPlayCommand>();

        public Vector3 rootMotionDeltaPos
        {
            get { return m_animator != null ? m_animator.deltaPosition : Vector3.zero; }
        }

        public Quaternion rootMotionTargetRotation
        {
            get { return m_animator != null ? m_animator.targetRotation : Quaternion.identity; }
        }

        public void SetAnimator(Animator animator)
        {
            m_animator = animator;
            if (m_animOverCtrl == null)
            {
                AnimatorOverrideController initOverCtrl = m_animator.runtimeAnimatorController as AnimatorOverrideController;
                if (initOverCtrl == null)
                {
                    Debug.LogError(string.Format("[CommonAnimatorController] m_animator.runtimeAnimatorController is not AnimatorOverrideController"));
                    return;
                }
                var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
                initOverCtrl.GetOverrides(overrides);
                m_animOverCtrl = new AnimatorOverrideController(initOverCtrl.runtimeAnimatorController);
                m_overrideKeyList.Clear();
                foreach (var kv in overrides)
                {
                    m_animOverCtrl[kv.Key.name] = kv.Value;
                    m_overrideKeyList.Add(kv.Key.name);
                }
            }
            if (m_animator != null && m_animator.runtimeAnimatorController != m_animOverCtrl)
            {
                m_animator.runtimeAnimatorController = m_animOverCtrl;
            }
        }

        public void AddStateGroup(int layerIndex, AnimatorStateType stateType, params string[] stateNames)
        {
            Layer layer = GetOrCreateLayer(layerIndex);
            StateGroup stateGroup = layer.GetOrCreateStateGroup(stateType);
            if (stateNames != null && stateNames.Length > 0)
            {
                for (int i = 0; i < stateNames.Length; ++i)
                {
                    stateGroup.stateNameList.AddNotContainsNotNull(stateNames[i]);
                }
            }
        }

        public void InitAnimationClipMap(List<AnimationClip> clipList)
        {
            foreach (var clip in clipList)
            {
                if (clip == null)
                {
                    continue;
                }
                if (!m_clipMap.ContainsKey(clip.name))
                {
                    m_clipMap.Add(clip.name, clip);
                }
            }
        }

        public void InitAnimatorOverrideController()
        {
        }

        public void Play(params AnimatorPlayCommand[] commands)
        {
            m_commandList.Clear();
            for (int i = 0; i < m_layerList.Count; ++i)
            {
                m_layerList[i].ResetPlayState();
            }
            AppendPlayCommandInternal(commands);
        }

        public void AppendPlayCommand(params AnimatorPlayCommand[] commands)
        {
            AppendPlayCommandInternal(commands);
        }

        private void AppendPlayCommandInternal(AnimatorPlayCommand[] commands)
        {
            if (commands == null || commands.Length == 0)
            {
                return;
            }
            for (int i = 0; i < commands.Length; ++i)
            {
                m_commandList.Add(commands[i]);
            }
            HandleNextCommand();
        }

        private void HandleNextCommand()
        {
            for (int i = 0; i < m_layerList.Count; ++i)
            {
                Layer layer = m_layerList[i];
                bool hasOnceCommand = HasOnceCommand(m_commandList, layer.index);
                if (layer.isPlaying)
                {
                    if (!layer.isCurStateLoop || !hasOnceCommand)
                    {
                        continue;
                    }
                }
                int index = 0;
                for (int j = 0; j < m_commandList.Count; ++j)
                {
                    AnimatorPlayCommand command = m_commandList[index];
                    if (command.layerIndex != layer.index)
                    {
                        index++;
                        continue;
                    }
                    m_commandList.RemoveAt(index);
                    if (AnimatorPlayUtility.CheckLoop(command.stateType) && hasOnceCommand)
                    {
                        continue;
                    }
                    if (PlayAnimationInternal(command))
                    {
                        break;
                    }
                }
            }
        }

        private bool HasOnceCommand(List<AnimatorPlayCommand> commandList, int layerIndex)
        {
            for (int i = 0; i < commandList.Count; ++i)
            {
                AnimatorPlayCommand command = commandList[i];
                if (command.layerIndex != layerIndex)
                {
                    continue;
                }
                if (!AnimatorPlayUtility.CheckLoop(command.stateType))
                {
                    return true;
                }
            }
            return false;
        }

        private bool PlayAnimationInternal(AnimatorPlayCommand command)
        {
            int layerIndex = command.layerIndex;
            AnimatorStateType stateType = command.stateType;
            float fadeTime = command.fadeTime;
            string clipName = command.name;
            Layer layer = GetLayer(layerIndex);
            if (layer == null)
            {
                Debug.LogError(string.Format("[CommonAnimatorController] Layer:{0} is not exist", layerIndex.ToString()));
                return false;
            }
            StateGroup stateGroup = layer.GetStateGroup(stateType);
            if (stateGroup == null)
            {
                Debug.LogError(string.Format("[CommonAnimatorController] StateGroup:{0} Layer:{1} is not exist", stateType.ToString(), layerIndex.ToString()));
                return false;
            }
            string stateName = stateGroup.TickAndGetNextStateName();
            if (!ReplaceOverrideClip(stateName, clipName))
            {
                return false;
            }
            if (m_animator != null)
            {
                m_animator.CrossFadeInFixedTime(stateName, fadeTime, layerIndex);
            }
            float clipLength = GetClip(clipName).length;
            layer.StartPlay(stateGroup, clipLength, command);
            return true;
        }

        private bool ReplaceOverrideClip(string keyName, string clipName)
        {
            if (!CanReplaceClip(keyName, clipName))
            {
                Debug.LogError(string.Format("[CommonAnimatorController] Cannot replace {0} to {1}", keyName, name));
                return false;
            }
            m_animOverCtrl[keyName] = GetClip(clipName);
            return true;
        }

        public bool CanReplaceClip(string keyName, string clipName)
        {
            if (!m_overrideKeyList.Contains(keyName))
            {
                return false;
            }
            if (GetClip(clipName) == null)
            {
                return false;
            }
            return true;
        }

        private Layer GetLayer(int layerIndex)
        {
            for (int i = 0; i < m_layerList.Count; ++i)
            {
                if (m_layerList[i].index == layerIndex)
                {
                    return m_layerList[i];
                }
            }
            return null;
        }

        private Layer GetOrCreateLayer(int layerIndex)
        {
            Layer layer = GetLayer(layerIndex);
            if (layer == null)
            {
                layer = new Layer();
                layer.index = layerIndex;
                layer.actionOnPlayEnd = OnLayerPlayStateEnd;
                m_layerList.Add(layer);
            }
            return layer;
        }

        private StateGroup GetStateGroup(int layerIndex, AnimatorStateType stateType)
        {
            Layer layer = GetLayer(layerIndex);
            if (layer != null)
            {
                return layer.GetStateGroup(stateType);
            }
            return null;
        }

        private AnimationClip GetClip(string name)
        {
            return m_clipMap.GetClassValue(name);
        }

        private void OnLayerPlayStateEnd(Layer layer)
        {
            Debug.Log(string.Format("{0},{1}", layer.index.ToString(), layer.curStateGroup.index));
            HandleNextCommand();
        }

        private void Update()
        {
            TimeSlice timeSlice = new TimeSlice()
            {
                deltaTime = Time.deltaTime,
                unscaledDeltaTime = Time.unscaledDeltaTime,
            };
            for (int i = 0; i < m_layerList.Count; ++i)
            {
                m_layerList[i].Tick(timeSlice);
            }
        }

        private class Layer
        {
            public int index;
            public Action<Layer> actionOnPlayEnd;

            private List<StateGroup> m_stateGroupList = new List<StateGroup>();
            private Timer m_stateTimer = new Timer();
            private Timer m_transitionTimer = new Timer();
            private StateGroup m_curStateGroup;
            private bool m_isPlaying;
            private AnimatorPlayCommand m_command;

            public bool isPlaying
            {
                get { return m_isPlaying; }
            }

            public StateGroup curStateGroup
            {
                get { return m_curStateGroup; }
            }

            public bool isCurStateLoop
            {
                get { return m_curStateGroup != null && m_curStateGroup.isLoop; }
            }

            public StateGroup GetStateGroup(AnimatorStateType stateType)
            {
                for (int i = 0; i < m_stateGroupList.Count; ++i)
                {
                    if (m_stateGroupList[i].stateType == stateType)
                    {
                        return m_stateGroupList[i];
                    }
                }
                return null;
            }

            public StateGroup GetOrCreateStateGroup(AnimatorStateType stateType)
            {
                StateGroup stateGroup = GetStateGroup(stateType);
                if (stateGroup == null)
                {
                    stateGroup = new StateGroup();
                    stateGroup.stateType = stateType;
                    m_stateGroupList.Add(stateGroup);
                }
                return stateGroup;
            }

            public void StartPlay(StateGroup stateGroup, float time, AnimatorPlayCommand command)
            {
                m_isPlaying = true;
                m_curStateGroup = stateGroup;
                m_command = command;
                if (m_curStateGroup.stateType == AnimatorStateType.Once)
                {
                    m_stateTimer.Start(time, OnOnceTimerEnd);
                }
            }

            public void ResetPlayState()
            {
                m_isPlaying = default;
                m_curStateGroup = default;
                m_command = default;
                m_stateTimer.Reset();
                m_transitionTimer.Reset();
            }

            public void Tick(TimeSlice timeSlice)
            {
                m_stateTimer.Tick(timeSlice);
                m_transitionTimer.Tick(timeSlice);
            }

            private void OnOnceTimerEnd()
            {
                m_isPlaying = false;
                Action temp = m_command.actionOnEnd;
                m_command = default;
                if (temp != null)
                {
                    temp();
                }
                if (actionOnPlayEnd != null)
                {
                    actionOnPlayEnd(this);
                }
            }
        }

        private class StateGroup
        {
            public AnimatorStateType stateType;
            public int index;
            public List<string> stateNameList = new List<string>();
            public float m_curTime;
            public float m_targetTime;
            public Action actionOnEnd;

            public bool isLoop
            {
                get { return stateType == AnimatorStateType.Loop; }
            }

            public string GetCurStateName()
            {
                return stateNameList.GetValueSafely(index);
            }

            public string TickAndGetNextStateName()
            {
                index++;
                if (index >= stateNameList.Count)
                {
                    index = 0;
                }
                return stateNameList.GetValueSafely(index);
            }

            public void Tick(float deltaTime)
            {
                if (stateType == AnimatorStateType.Once)
                {
                    m_curTime += deltaTime;
                    if (m_curTime > m_targetTime)
                    {
                        if (actionOnEnd != null)
                        {
                            Action temp = actionOnEnd;
                            actionOnEnd = null;
                            temp();
                        }
                    }
                }
            }
        }
    }
}
