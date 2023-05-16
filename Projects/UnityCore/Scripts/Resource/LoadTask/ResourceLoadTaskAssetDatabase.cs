using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonMoose.Core
{
    public class ResourceLoadTaskAssetDatabase<T> : ResourceLoadTask where T : UnityEngine.Object
    {
        protected override bool CheckLoadEnd()
        {
            return true;
        }

        protected override void OnLoadStart()
        {
            base.OnLoadStart();
            resourceObj = ResourceUtility.LoadFormAssetDatabaseSync<T>(loadPath, subName);
        }
    }
}
