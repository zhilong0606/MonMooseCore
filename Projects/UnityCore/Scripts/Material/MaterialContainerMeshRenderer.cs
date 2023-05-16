using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonMoose.Core
{
    public class MaterialContainerMeshRenderer : MaterialContainerBase
    {
        [SerializeField]
        private MeshRenderer m_meshRenderer;

        protected override Material curMaterial
        {
            get { return meshRenderer != null ? meshRenderer.material : null; }
            set
            {
                if (meshRenderer != null)
                {
                    meshRenderer.material = value;
                }
            }
        }

        protected MeshRenderer meshRenderer
        {
            get
            {
                if (m_meshRenderer == null)
                {
                    m_meshRenderer = GetComponent<MeshRenderer>();
                }
                return m_meshRenderer;
            }
        }
    }
}
