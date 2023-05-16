using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonMoose.Core
{
    public class UIController : MonoBehaviour
    {
        protected bool m_isInitialized;
        protected UIController m_parent;
        protected List<UIController> m_childCtrlList = new List<UIController>();
        protected List<UIController> m_prepareCtrlList = new List<UIController>();

        private SceneLayerCanvas m_canvas;

        public virtual SceneLayerCanvas canvas
        {
            get
            {
                if (CheckCanvas())
                {
                    m_canvas = null;
                }
                if (m_canvas == null)
                {
                    m_canvas = FindCanvas();
                }
                return m_canvas;
            }
        }

        public void Init(UIController parent)
        {
            if (m_isInitialized)
            {
                return;
            }
            m_isInitialized = true;
            BindGroups();
            for (int i = 0; i < m_prepareCtrlList.Count; ++i)
            {
                m_prepareCtrlList[i].Init(this);
            }
            BindViews();
            OnInit();
            BindParent(parent);
            m_prepareCtrlList.Clear();
        }

        public void UnInit()
        {
            if (!m_isInitialized)
            {
                return;
            }
            m_isInitialized = false;
            OnUnInit();
            List<UIController> ctrlList = new List<UIController>();
            ctrlList.AddRange(m_childCtrlList);
            for (int i = 0; i < ctrlList.Count; ++i)
            {
                ctrlList[i].UnInit();
            }
            m_childCtrlList.Clear();
            BindParent(null);
            m_canvas = null;
        }

        public void BindGroups()
        {
            UIBindGroup bindGroup = GetComponent<UIBindGroup>();
            if (bindGroup != null)
            {
                OnBindSubGroupsByBindGroup(bindGroup);
            }
        }

        public void BindViews()
        {
            UIBindGroup bindGroup = GetComponent<UIBindGroup>();
            if (bindGroup != null)
            {
                OnBindViewsByBindGroup(bindGroup);
            }
            for (int i = 0; i < m_childCtrlList.Count; ++i)
            {
                m_childCtrlList[i].BindViews();
            }
        }

        public void BindParent(UIController parent)
        {
            if (parent == m_parent || parent == this)
            {
                return;
            }
            if (m_parent != null)
            {
                m_parent.RemoveChildController(this);
            }
            m_parent = parent;
            if (m_parent != null)
            {
                m_parent.AddChildController(this);
            }
        }

        public void AddChildController(UIController child)
        {
            if (child != this && child != null)
            {
                m_childCtrlList.AddNotContains(child);
            }
        }

        public void RemoveChildController(UIController child)
        {
            m_childCtrlList.Remove(child);
        }

        protected void AddBindComponent(UIBindItemInfo itemInfo, Type type)
        {
            if (itemInfo.bindObj == null)
            {
                return;
            }
            UIController ctrl = itemInfo.bindObj.AddComponent(type) as UIController;
            m_prepareCtrlList.AddNotContainsNotNull(ctrl);
        }

        private SceneLayerCanvas FindCanvas()
        {
            if (m_canvas == null)
            {
                m_canvas = SceneLayerManager.instance.FindLayer(transform) as SceneLayerCanvas;
            }
            return m_canvas;
        }

        private bool CheckCanvas()
        {
            if (m_canvas != null)
            {
                return SceneLayerManager.instance.CheckLayer(transform, m_canvas);
            }
            return false;
        }

        protected virtual void OnInit() { }
        protected virtual void OnUnInit() { }
        protected virtual void OnBindSubGroupsByBindGroup(UIBindGroup group) { }
        protected virtual void OnBindViewsByBindGroup(UIBindGroup group) { }
    }
}
