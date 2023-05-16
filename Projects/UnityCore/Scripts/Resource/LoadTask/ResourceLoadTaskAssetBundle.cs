using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonMoose.Core
{
    public class ResourceLoadTaskAssetBundle<T> : ResourceLoadTask where T : UnityEngine.Object
    {
        protected override bool CheckLoadEnd()
        {
            return true;
        }
    }
}
