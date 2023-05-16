using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MonMoose.Core
{
    public class SceneLayerCanvas : SceneLayerBase
    {
        [SerializeField]
        private Canvas m_canvas;
        [SerializeField]
        private GraphicRaycaster m_raycaster;

        [SerializeField]
        private int m_preCameraSpace = 5;
        [SerializeField]
        private int m_postCameraSpace = 5;

        public override Camera worldCamera
        {
            get
            {
                if (m_canvas == null)
                {
                    m_canvas = GetComponent<Canvas>();
                }
                if (m_canvas != null)
                {
                    return m_canvas.worldCamera;
                }
                return null;
            }
            set
            {
                if (m_canvas == null)
                {
                    m_canvas = GetComponent<Canvas>();
                }
                if (m_canvas != null)
                {
                    m_canvas.worldCamera = value;
                }
            }
        }

        public override BaseRaycaster raycaster
        {
            get
            {
                if (m_raycaster == null)
                {
                    m_raycaster = GetComponent<GraphicRaycaster>();
                }
                return m_raycaster;
            }
        }

        public int sortingOrder
        {
            set
            {
                if (m_canvas == null)
                {
                    m_canvas = GetComponent<Canvas>();
                }
                if (m_canvas != null)
                {
                    m_canvas.sortingOrder = value;
                }
            }
        }

        public int planeDistance
        {
            set
            {
                if (m_canvas == null)
                {
                    m_canvas = GetComponent<Canvas>();
                }
                if (m_canvas != null)
                {
                    m_canvas.planeDistance = value;
                }
            }
        }

        public int preCameraSpace
        {
            get { return m_preCameraSpace; }
        }

        public int postCameraSpace
        {
            get { return m_postCameraSpace; }
        }

        public override void ClearOnPop()
        {
            worldCamera = null;
        }

#if UNITY_EDITOR
        private void Reset()
        {
            m_canvas = GetComponent<Canvas>();
            m_raycaster = GetComponent<GraphicRaycaster>();
            UIMultiLayerConfig config = GetComponent<UIMultiLayerConfig>();
            if (config == null)
            {
                config = GetComponentInParent<UIMultiLayerConfig>();
            }
            if (config == null)
            {
                config = gameObject.AddComponent<UIMultiLayerConfig>();
            }
            for (int i = config.layerList.Count - 1; i>=0; --i)
            {
                if (config.layerList[i] == null)
                {
                    config.layerList.RemoveAt(i);
                }
            }
            if (!config.layerList.Contains(this))
            {
                config.layerList.Add(this);
            }
        }
#endif
    }
}
