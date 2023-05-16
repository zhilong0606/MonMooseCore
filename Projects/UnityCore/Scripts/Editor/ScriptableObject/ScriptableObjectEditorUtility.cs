using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MonMoose.Core
{
    public static class ScriptableObjectEditorUtility
    {
        public static T GetInstance<T>() where T : ScriptableObject
        {
            var pathes = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));
            if (pathes.Length == 0)
            {
                Debug.LogError(string.Format("[ScriptableObjectEditorUtility] GetInstance: Cannot Find {0}", typeof(T).Name));
                return null;
            }
            if (pathes.Length > 1)
            {
                Debug.LogError(string.Format("[ScriptableObjectEditorUtility] GetInstance: More than One Instance {0}", typeof(T).Name));
            }
            return AssetDatabase.LoadAssetAtPath<T>(pathes[0]);
        }
    }
}
