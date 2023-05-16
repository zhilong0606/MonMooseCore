using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MonMoose.Core
{
    public abstract class SceneLayerBase : MonoBehaviour
    {
        public abstract Camera worldCamera { get; set; }

        public abstract BaseRaycaster raycaster { get; }

        [SerializeField]
        protected ESceneLayerPriority m_priority = ESceneLayerPriority.Normal;

        public ESceneLayerPriority priority
        {
            get { return m_priority; }
        }

        public virtual void ClearOnPop()
        {

        }
    }
}
