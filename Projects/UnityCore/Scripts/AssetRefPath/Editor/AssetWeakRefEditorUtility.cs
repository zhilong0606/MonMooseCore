using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
            GetPath(property).stringValue = path;
            GetGuid(property).stringValue = guid;
        }

        public static T GetAssetByWeakRef<T>(AssetWeakRef refPath) where T : UnityEngine.Object
        {
            bool needUpdate;
            return GetAssetByWeakRef<T>(refPath, out needUpdate);
        }

        public static T GetAssetByWeakRef<T>(AssetWeakRef refPath, out bool needUpdate) where T : UnityEngine.Object
        {
            string path = refPath.path;
            string guid = refPath.guid;
            return GetAssetByWeakRef<T>(path, guid, out needUpdate);
        }

        public static T GetAssetByWeakRef<T>(SerializedProperty property, out bool needUpdate) where T : UnityEngine.Object
        {
            string path = GetPath(property).stringValue;
            string guid = GetGuid(property).stringValue;
            return GetAssetByWeakRef<T>(path, guid, out needUpdate);
        }

        public static T GetAssetByWeakRef<T>(string path, string guid, out bool needUpdate) where T : UnityEngine.Object
        {
            T assetObjByPath = GetAssetByPath<T>(path);
            T assetObjByGuid = GetAssetByGuid<T>(guid);
            needUpdate = assetObjByPath != assetObjByGuid;
            if (assetObjByGuid != null)
            {
                return assetObjByGuid;
            }
            if (assetObjByPath != null)
            {
                return assetObjByPath;
            }
            return null;
        }

        public static T PropertyField<T>(string name, AssetWeakRef weakRef, ref bool isDirty, Action<AssetWeakRef> setter, params GUILayoutOption[] options) where T : UnityEngine.Object
        {
            bool needUpdate;
            T asset = GetAssetByWeakRef<T>(weakRef, out needUpdate);
            T newAsset = EditorGUILayout.ObjectField(name, asset, typeof(T), false, options) as T;
            if (newAsset != asset || needUpdate)
            {
                asset = newAsset;
                if (setter != null)
                {
                    setter(GetAssetWeakRef(asset));
                }
                isDirty = true;
            }
            return asset;
        }

        public static SerializedProperty GetPath(SerializedProperty sp)
        {
            return sp.FindPropertyRelative("path");
        }


        public static SerializedProperty GetGuid(SerializedProperty sp)
        {
            return sp.FindPropertyRelative("guid");
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
