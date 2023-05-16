using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace MonMoose.Core
{
    public static class AssetPathUtility
    {
        public static string GetAssetPath(FileInfo fileInfo)
        {
            return GetAssetPathByFullPath(fileInfo.FullName);
        }

        public static string GetAssetFolderPathByFullPath(string fullPath)
        {
            return string.Format("Assets/{0}", FilePathUtility.NormalizePath(fullPath).Substring(Application.dataPath.Length + 1)).TrimEnd('/');
        }

        public static string GetAssetPathByFullPath(string fullPath)
        {
            return string.Format("Assets/{0}", FilePathUtility.NormalizePath(fullPath).Substring(Application.dataPath.Length + 1));
        }
#if UNITY_EDITOR
        public static string GetClassFullPath(Type type)
        {
            return GetClassPath(type, true);
        }

        public static string GetClassAssetPath(Type type)
        {
            return GetClassPath(type, false);
        }

        public static string GetClassPath(Type type, bool isFullPath)
        {
            string path = UnityEditor.AssetDatabase.FindAssets("t:Script")
                .Where(v => Path.GetFileNameWithoutExtension(UnityEditor.AssetDatabase.GUIDToAssetPath(v)) == type.Name)
                .Select(UnityEditor.AssetDatabase.GUIDToAssetPath)
                .FirstOrDefault();
            if (isFullPath)
            {
                return FilePathUtility.Concat(Application.dataPath, path, "Assets");
            }
            return path;
        }
#endif
    }
}