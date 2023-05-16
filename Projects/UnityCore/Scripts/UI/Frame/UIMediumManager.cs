using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonMoose.Core
{
    public class UIMediumManager : Singleton<UIMediumManager>
    {
        private List<UIMedium> m_mediumList = new List<UIMedium>();
        private List<UIMedium> m_tempMediumList = new List<UIMedium>();
        private List<UIMediumConfig> m_mediumConfigList = new List<UIMediumConfig>();
        private const int m_stackSize = 1;
        private const float m_openedTimeThreshold = 1f;

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

        public void RegisterConfig(UIMediumConfig config)
        {
            m_mediumConfigList.Add(config);
        }

        public UIMedium Open(UIMediumId mediumId, bool isAsync, bool immediately = false, Action<UIMedium> actionOnOpenEnd = null)
        {
            UIMediumContext context = new UIMediumContext(mediumId);
            return Open(context, isAsync, immediately, actionOnOpenEnd);
        }

        public UIMedium Open(UIMediumContext context, bool isAsync, bool immediately = false, Action<UIMedium> actionOnOpenEnd = null)
        {
            UIMedium medium = GetOrCreateMedium(context.mediumId);
            medium.Open(context, isAsync, immediately, actionOnOpenEnd);
            return medium;
        }

        public void Close(UIMediumId mediumId, bool immediately = false, bool destroy = false, Action<UIMedium> actionOnCloseEnd = null)
        {
            UIMedium medium = GetOrCreateMedium(mediumId);
            medium.Close(immediately, destroy, actionOnCloseEnd);
        }

        public void CloseImmediatelyExcept(UIMediumTagId[] tags, bool destroy)
        {
            for (int i = 0; i < m_mediumList.Count; ++i)
            {
                UIMedium medium = m_mediumList[i];
                if (!medium.HasTags(tags))
                {
                    medium.Close(true, destroy);
                }
            }
        }

        public void RefreshStack()
        {
            for (int i = 0; i < m_mediumList.Count; ++i)
            {
                UIMedium medium = m_mediumList[i];
                if (!medium.stackOptimizable)
                {
                    continue;
                }
                if (medium.mediumState == UIMediumState.Opened)
                {
                    m_tempMediumList.Add(medium);
                }
            }
            m_tempMediumList.Sort(OnSortMediumByOpenedTimeNear);
            for (int i = m_stackSize; i < m_tempMediumList.Count; ++i)
            {
                m_tempMediumList[i].Close(true, true);
            }
            m_tempMediumList.Clear();
        }

        public UIMedium GetOrCreateMedium(UIMediumId id)
        {
            UIMedium medium = GetMedium(id);
            if (medium == null)
            {
                medium = CreateMedium(id);
            }
            return medium;
        }

        public UIMedium GetMedium(UIMediumId mediumId)
        {
            for (int i = 0; i < m_mediumList.Count; ++i)
            {
                if (m_mediumList[i].mediumId == mediumId)
                {
                    return m_mediumList[i];
                }
            }
            return null;
        }

        public bool HasMedium(UIMediumId mediumId)
        {
            return GetMedium(mediumId) != null;
        }

        public UIMedium CreateMedium(UIMediumId mediumId)
        {
            UIMediumConfig config = GetMediumConfig(mediumId);
            if (config == null)
            {
                return null;
            }
            UIMedium medium = Activator.CreateInstance(config.classType) as UIMedium;
            if (medium == null)
            {
                return null;
            }
            medium.Init(config);
            m_mediumList.Add(medium);
            return medium;
        }

        public UIMediumConfig GetMediumConfig(UIMediumId mediumId)
        {
            for (int i = 0; i < m_mediumConfigList.Count; ++i)
            {
                if (m_mediumConfigList[i].mediumId == mediumId)
                {
                    return m_mediumConfigList[i];
                }
            }
            return null;
        }

        private void OnTick(TimeSlice timeSlice)
        {
            for (int i = 0; i < m_mediumList.Count; ++i)
            {
                m_mediumList[i].Tick(timeSlice);
            }
        }

        private int OnSortMediumByOpenedTimeNear(UIMedium m1, UIMedium m2)
        {
            return m2.openedTime.CompareTo(m1.openedTime);
        }
    }
}
