using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MonMoose.Core
{
    public static class UIMediumConfigEditorUtility
    {
        public static MonoScript GetMonoScript(UIMediumConfig config)
        {
            if (config == null)
            {
                return null;
            }
            return AssetWeakRefEditorUtility.GetAssetByWeakRef<MonoScript>(config.scriptWeakRef);
        }

        public static GameObject GetPrefab(UIViewConfig config)
        {
            if (config == null)
            {
                return null;
            }
            return AssetWeakRefEditorUtility.GetAssetByWeakRef<GameObject>(config.prefabWeakRef);
        }

        public static MonoScript GetMonoScript(UIViewConfig config)
        {
            GameObject prefab = GetPrefab(config);
            if (prefab != null)
            {
                UIBindGroup bindGroup = prefab.GetComponent<UIBindGroup>();
                if (bindGroup != null)
                {
                    return UIBindEditorUtility.GetMonoScriptBindGroup(bindGroup);
                }
            }
            return null;
        }
    }
}
