using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonMoose.Core
{
    public abstract class MaterialContainerBase : Inventory<Material>
    {
        protected abstract Material curMaterial { get; set; }

        public void ChangeMaterial(int index)
        {
            if (index >= 0 && index < Count)
            {
                Material mat = this[index];
                if (mat != curMaterial)
                {
                    curMaterial = mat;
                }
            }
        }
    }
}
