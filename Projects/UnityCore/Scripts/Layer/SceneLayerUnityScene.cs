using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MonMoose.Core
{
    public class SceneLayerUnityScene : SceneLayerBase
    {
        private Camera m_worldCamera;
        private PhysicsRaycaster m_raycaster;

        public override Camera worldCamera
        {
            get { return m_worldCamera; }
            set { m_worldCamera = value; }
        }

        public override BaseRaycaster raycaster
        {
            get { return m_raycaster; }
        }

        public void Init(Camera worldCamera, ESceneLayerPriority priority)
        {
            m_priority = priority;
            m_worldCamera = worldCamera;
            m_raycaster = worldCamera.GetComponent<PhysicsRaycaster>();
        }
    }
}
