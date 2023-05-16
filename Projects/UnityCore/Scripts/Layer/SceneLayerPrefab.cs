using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MonMoose.Core
{
    public class SceneLayerPrefab : SceneLayerBase
    {
        [SerializeField]
        private Camera m_worldCamera;
        [SerializeField]
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
    }
}
