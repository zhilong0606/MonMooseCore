using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MonMoose.Core
{
    public static class AssetWeakRefEditorUtility
    {
        public static AssetWeakRef GetAssetWeakRef(UnityEngine.Object assetObj)
        {
            if (assetObj == null)
            {
                return new AssetWeakRef(string.Empty, string.Empty);
            }
            string path = AssetDatabase.GetAssetPath(assetObj);
            string guid = AssetDatabase.GUIDFromAssetPath(path).ToString();
            return new AssetWeakRef(path, guid);
        }

        public static void SetAssetWeakRef(SerializedProperty property, UnityEngine.Object assetObj)
        {
            string path = string.Empty;
            string guid = string.Empty;
            if (assetObj != null)
            {
                path = AssetDatabase.GetAssetPath(assetObj);
                guid = AssetDatabase.GUIDFromAssetPath(path).ToString();
            }
            property.FindPropertyRelative("path").stringValue = path;
            property.FindPropertyRelative("guid").stringValue = guid;
        }

        public static T GetAssetByWeakRef<T>(AssetWeakRef refPath) where T : UnityEngine.Object
        {
            bool needUpdate;
            return GetAssetByWeakRef<T>(refPath, out needUpdate);
        }

        public static T GetAssetByWeakRef<T>(AssetWeakRef refPath, out bool needUpdate) where T : UnityEngine.Object
        {
            T assetObj = GetAssetByPath<T>(refPath.path);
            needUpdate = false;
            if (assetObj == null)
            {
                assetObj = GetAssetByGuid<T>(refPath.guid);
                needUpdate = true;
            }
            return assetObj;
        }

        public static T GetAssetByWeakRef<T>(SerializedProperty property, out bool needUpdate) where T : UnityEngine.Object
        {
            T assetObj = GetAssetByPath<T>(property.FindPropertyRelative("path").stringValue);
            needUpdate = false;
            if (assetObj == null)
            {
                assetObj = GetAssetByGuid<T>(property.FindPropertyRelative("guid").stringValue);
                if (assetObj != null)
                {
                    needUpdate = true;
                }
            }
            return assetObj;
        }

        private static T GetAssetByPath<T>(string path) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }

        private static T GetAssetByGuid<T>(string guid) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(guid))
            {
                return null;
            }
            return GetAssetByPath<T>(AssetDatabase.GUIDToAssetPath(guid));
        }
    }
}
