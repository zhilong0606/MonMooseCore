using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonMoose.Core
{
    public class SceneLayerGroup : ClassPoolObj
    {
        public int sortingOrder;
        public List<SceneLayerBase> layerList = new List<SceneLayerBase>();

        public override void OnRelease()
        {
            sortingOrder = default;
            layerList.Clear();
            base.OnRelease();
        }
    }
}
