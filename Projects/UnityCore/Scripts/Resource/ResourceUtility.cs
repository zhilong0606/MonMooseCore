using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonMoose.Core
{
    public static class ResourceUtility
    {
        public static T LoadFormAssetDatabaseSync<T>(string resourceLoadPath, string resourceSubName) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(resourceSubName))
            {
                return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(resourceLoadPath);
            }
            UnityEngine.Object[] resourceObjs = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(resourceLoadPath);
            int count = resourceObjs.Length;
            for (int i = 0; i < count; ++i)
            {
                T resourceObj = resourceObjs[i] as T;
                if (resourceObj != null && resourceObj.name == resourceSubName)
                {
                    return resourceObj;
                }
            }
#endif
            return null;
        }
    }
}
