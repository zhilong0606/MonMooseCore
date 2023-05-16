using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonMoose.Core
{
    public class SceneLayerManager : MonoSingleton<SceneLayerManager>
    {
        [SerializeField]
        private GameObject m_layerRoot;
        [SerializeField]
        private GameObjectPool m_canvasCameraPool;

        private List<SceneLayerBase> m_layerStack = new List<SceneLayerBase>();
        private int m_cameraStartDepth = 10;
        private int m_cameraSpace = 10;
        private int m_sortingLayerSpace = 100;
        private bool m_isLayerSortDirty;

        protected override void OnInit()
        {
            base.OnInit();
            m_canvasCameraPool.Init();
            TickManager.instance.RegisterGlobalTick(OnTick);
        }

        protected override void OnUnInit()
        {
            base.OnUnInit();
            TickManager.instance.UnRegisterGlobalTick(OnTick);
        }

        public void PushLayer(SceneLayerBase layer, bool needSortImmediately)
        {
            if (layer == null)
            {
                return;
            }
            if (m_layerStack.Contains(layer))
            {
                return;
            }
            int insertIndex = -1;
            int layerCount = m_layerStack.Count;
            for (int i = 0; i < layerCount; ++i)
            {
                if(m_layerStack[i].priority > layer.priority)
                {
                    insertIndex = i;
                    break;
                }
            }
            if (!(layer is SceneLayerUnityScene))
            {
                layer.gameObject.SetParent(m_layerRoot);
            }
            if (insertIndex < 0)
            {
                m_layerStack.Add(layer);
            }
            else
            {
                m_layerStack.Insert(insertIndex, layer);
            }
            SortAllLayers(needSortImmediately);
        }

        public void PushLayerList(IEnumerable<SceneLayerBase> layerList, bool needSortImmediately)
        {
            foreach (SceneLayerBase layer in layerList)
            {
                PushLayer(layer, false);
            }
            SortAllLayers(needSortImmediately);
        }

        public void PopLayer(SceneLayerBase layer, bool needSortImmediately)
        {
            if (layer == null)
            {
                return;
            }
            layer.ClearOnPop();
            m_layerStack.Remove(layer);
            SortAllLayers(needSortImmediately);
        }

        public void PopLayerList(IEnumerable<SceneLayerBase> layerList, bool needSortImmediately)
        {
            foreach (SceneLayerBase layer in layerList)
            {
                PopLayer(layer, false);
            }
            SortAllLayers(needSortImmediately);
        }

        public bool CheckLayer(Transform trans, SceneLayerBase layer)
        {
            while (trans != null)
            {
                if (layer.transform == trans)
                {
                    return true;
                }
                trans = trans.parent;
            }
            return false;
        }

        public SceneLayerBase FindLayer(Transform trans)
        {
            int count = m_layerStack.Count;
            while (trans != null)
            {
                for (int i = 0; i < count; ++i)
                {
                    if (m_layerStack[i].transform == trans)
                    {
                        return m_layerStack[i];
                    }
                }
                trans = trans.parent;
            }
            return null;
        }

        private void SetLayerSortDirty()
        {
            m_isLayerSortDirty = true;
        }

        private void OnTick(TimeSlice timeSlice)
        {
            if (m_isLayerSortDirty)
            {
                SortAllLayersImmediately();
                m_isLayerSortDirty = false;
            }
        }

        private void SortAllLayers(bool needSortImmediately)
        {
            if (needSortImmediately)
            {
                SortAllLayersImmediately();
            }
            else
            {
                SetLayerSortDirty();
            }
        }

        private void SortAllLayersImmediately()
        {
            Camera canvasCamera = null;
            float cameraPosZ = 0f;
            int cameraDepth = m_cameraStartDepth;
            int sortingOrder = 0;
            m_canvasCameraPool.ReleaseAll();
            int count = m_layerStack.Count;
            int canvasCameraDistance = 0;
            for (int i = 0; i < count; ++i)
            {
                SceneLayerBase layer = m_layerStack[i];
                SceneLayerCanvas canvasLayer = layer as SceneLayerCanvas;
                if (canvasLayer != null)
                {
                    if (canvasCamera == null)
                    {
                        cameraPosZ += canvasCameraDistance + m_cameraSpace;
                        canvasCameraDistance = 0;
                        sortingOrder = 0;
                        canvasCamera = m_canvasCameraPool.FetchComponent<Camera>();
                        canvasCamera.depth = cameraDepth;
                        canvasCamera.transform.position = Vector3.forward * cameraPosZ;
                        cameraDepth++;
                    }
                    canvasLayer.worldCamera = canvasCamera;
                    canvasLayer.sortingOrder = sortingOrder * m_sortingLayerSpace;
                    canvasCameraDistance += canvasLayer.preCameraSpace;
                    canvasLayer.planeDistance = canvasCameraDistance;
                    canvasCameraDistance += canvasLayer.postCameraSpace;
                    sortingOrder++;
                    canvasCamera.farClipPlane = canvasCameraDistance;
                }
                else
                {
                    layer.worldCamera.depth = cameraDepth;
                    cameraDepth++;
                    canvasCamera = null;
                }
            }
        }
    }
}
