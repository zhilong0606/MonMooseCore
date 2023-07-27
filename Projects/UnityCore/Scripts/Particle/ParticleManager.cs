using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonMoose.Core;

namespace MonMoose.GameLogic
{
    public class ParticleManager : Singleton<ParticleManager>
    {
        private ParticleDataInventory m_battleCommonInventory;
        private List<ParticleRuntimeInfo> m_runtimeInfoList = new List<ParticleRuntimeInfo>();

        protected override void OnInit()
        {
            base.OnInit();
            m_battleCommonInventory = ResourceManager.instance.LoadSync<ParticleDataInventory>("Assets/Res/Fx/Battle/Common/BattleCommonParticleDataInventory.asset");
            TickManager.instance.RegisterGlobalTick(OnTick);
        }

        protected override void OnUnInit()
        {
            base.OnUnInit();
            TickManager.instance.UnRegisterGlobalTick(OnTick);
        }

        public GameObject Spawn(int id, GameObject parent, Action actionOnEnd = null)
        {
            string path = GetParticlePath(id);
            return Spawn(path, parent, actionOnEnd);
        }

        public GameObject Spawn(string path, GameObject parent, Action actionOnEnd = null)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            GameObject prefab = ResourceManager.instance.LoadSync<GameObject>(path);
            ParticleConfig config = prefab.GetComponent<ParticleConfig>();
            if (config == null)
            {
                return null;
            }
            GameObject particleObj = GameObjectPoolManager.instance.Fetch(path);
            particleObj.SetParent(parent);
            particleObj.transform.localScale = Vector3.one;
            particleObj.transform.localRotation = Quaternion.identity;
            particleObj.transform.localPosition = Vector3.zero;
            ParticleRuntimeInfo runtimeInfo = ClassPoolManager.instance.Fetch<ParticleRuntimeInfo>();
            runtimeInfo.go = particleObj;
            runtimeInfo.config = config;
            runtimeInfo.actionOnEnd = actionOnEnd;
            m_runtimeInfoList.Add(runtimeInfo);
            return particleObj;
        }

        protected GameObject GetParticlePrefab(int id)
        {
            string path = GetParticlePath(id);
            if (!string.IsNullOrEmpty(path))
            {
                return ResourceManager.instance.LoadSync<GameObject>(path);
            }
            return null;
        }

        protected string GetParticlePath(int id)
        {
            ParticleData data = GetParticleData(id);
            if (data != null)
            {
                return data.effect.path;
            }
            return string.Empty;
        }

        protected ParticleData GetParticleData(int id)
        {
            foreach (var particleData in m_battleCommonInventory.particleList)
            {
                if (particleData.id == id)
                {
                    return particleData;
                }
            }
            return null;
        }

        private void OnTick(TimeSlice timeSlice)
        {
            for (int i = 0; i < m_runtimeInfoList.Count;)
            {
                var info = m_runtimeInfoList[i];
                info.Tick(timeSlice);
                if (info.isOver)
                {
                    m_runtimeInfoList.RemoveAt(i);
                    info.actionOnEnd.InvokeSafely();
                    info.Release();
                    continue;
                }
                i++;
            }
        }
    }
}
