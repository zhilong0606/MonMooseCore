using System;
using System.Collections;
using System.Collections.Generic;

namespace MonMoose.Core
{
    public class ResourceLoaderAssetDatabase : ResourceLoader
    {
        protected override T OnLoadSync<T>(string resourceLoadPath, string resourceSubName)
        {
            return ResourceUtility.LoadFormAssetDatabaseSync<T>(resourceLoadPath, resourceSubName);
        }

        protected override ResourceLoadTask CreateLoadTask<T>()
        {
            return ClassPoolManager.instance.Fetch<ResourceLoadTaskAssetDatabase<T>>();
        }
    }
}