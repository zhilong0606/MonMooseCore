#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace MonMoose.Core
{
    public static class CommonTool
    {
        private static readonly List<string> m_resoucePathPrefixStrList = new List<string>()
        {
        };

        //[MenuItem("Assets/CommonTool/Get Resource Path %k", false, 1)]
        //static void GetAssetPath()
        //{
        //    string path = string.Empty;
        //    if (Selection.activeObject != null)
        //    {
        //        Object selectedObj = Selection.activeObject;
        //        if (AssetDatabase.Contains(selectedObj))
        //        {
        //            path = AssetDatabase.GetAssetPath(selectedObj);
        //            for (int i = 0; i < m_resoucePathPrefixStrList.Count; ++i)
        //            {
        //                string prefix = m_resoucePathPrefixStrList[i];

        //                if (path.StartsWith(prefix))
        //                {
        //                    path = path.Substring(prefix.Length);
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //    EditorGUIUtility.systemCopyBuffer = path;
        //}

        [MenuItem("Assets/CommonTool/Collect AnimationClips %j", false, 2)]
        static void CollectAnimationClips()
        {
            AnimationCollectionInspector.collectedAnimationClipList.Clear();
            if (Selection.objects == null || Selection.objects.Length == 0)
            {
                return;
            }
            List<AnimationClip> clipList = new List<AnimationClip>();
            foreach (var selectObj in Selection.objects)
            {
                CollectAnimationClips(selectObj, clipList);
            }
            clipList = clipList.Distinct().ToList();
            AnimationCollectionInspector.collectedAnimationClipList.AddRange(clipList);
        }

        private static void CollectAnimationClips(UnityEngine.Object selectObj, List<AnimationClip> clipList)
        {
            string assetPath = AssetDatabase.GetAssetPath(selectObj);
            if (selectObj is AnimationClip)
            {
                clipList.Add(selectObj as AnimationClip);
            }
            if (selectObj is GameObject)
            {
                var clipListInFbx = AssetDatabase.LoadAllAssetsAtPath(assetPath)
                    .Where(a => a is AnimationClip && !a.hideFlags.HasFlag(HideFlags.HideInHierarchy) && !a.hideFlags.HasFlag(HideFlags.HideInInspector))
                    .Cast<AnimationClip>().ToList();
                clipList.AddRange(clipListInFbx);
            }
            else if (selectObj is DefaultAsset)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(assetPath);
                if (directoryInfo.Exists)
                {
                    var fileInfos = directoryInfo.GetFiles("*.fbx", SearchOption.AllDirectories);
                    foreach (var fileInfo in fileInfos)
                    {
                        string fileAssetPath = assetPath + fileInfo.FullName.Substring(directoryInfo.FullName.Length);
                        UnityEngine.Object fileObj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(fileAssetPath);
                        CollectAnimationClips(fileObj, clipList);
                    }
                }
            }
        }
    }
}
#endif