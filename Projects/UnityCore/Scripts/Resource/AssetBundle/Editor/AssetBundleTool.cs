#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace MonMoose.Core
{
    public static class AssetBundleTool
    {
        private static List<AssetBundleBuild> m_bundleBuildList = new List<AssetBundleBuild>();
        private static string m_resFolderAssetPath = "Asset/Res";

        private static List<string> m_assetExtensionList = new List<string>()
        {
            ".prefab",
            ".bytes",
            ".asset",
            ".png"
        };

        [MenuItem("Tools/Asset Bundle/Build Asset Bundle")]
        static void BuildAssetBundle()
        {
            m_bundleBuildList.Clear();
            List<string> pathList = AssetDatabase.FindAssets(string.Format("t:{0}", nameof(AssetBundleDescription)))
                .Select(AssetDatabase.GUIDToAssetPath).ToList();
            pathList.Sort();
            AssetBundleInfoCollection collection = CreateBundleInfoContainer();
            collection.infoList.Clear();
            foreach (string path in pathList)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(FilePathUtility.GetFileFolderPath(path));
                if (!directoryInfo.Exists)
                {
                    continue;
                }
                string assetFolderPath = FilePathUtility.GetFileFolderPath(path);
                List<string> assetPathList = new List<string>();
                string assetBundleName = GetAssetBundleName(assetFolderPath);
                string assetBundleVariant = "ab".ToLower();
                AssetBundleInfo assetBundleInfo = new AssetBundleInfo();
                collection.infoList.Add(assetBundleInfo);
                assetBundleInfo.bundlePath = AssetPathUtility.GetAssetFolderPathByFullPath(string.Format("{0}/{1}.{2}", Application.streamingAssetsPath, assetBundleName, assetBundleVariant));
                foreach (FileInfo fileInfo in directoryInfo.GetFiles("*.*", SearchOption.AllDirectories))
                {
                    if (!m_assetExtensionList.Contains(fileInfo.Extension))
                    {
                        continue;
                    }
                    string assetPath = AssetPathUtility.GetAssetPath(fileInfo);
                    if (path == assetPath)
                    {
                        ResetAssetImporter(assetPath);
                        continue;
                    }
                    SetAssetImporter(assetPath, assetBundleName, assetBundleVariant);
                    assetPathList.Add(assetPath);
                    assetBundleInfo.assetPathList.Add(assetPath);
                }

                AssetBundleBuild build = new AssetBundleBuild();
                build.assetNames = assetPathList.ToArray();
                build.assetBundleName = assetBundleName;
                build.assetBundleVariant = assetBundleVariant;
                m_bundleBuildList.Add(build);

            }
            EditorUtility.SetDirty(collection);
            AssetDatabase.SaveAssets();

            string streamingAssetsPath = Application.streamingAssetsPath;
            if (!Directory.Exists(streamingAssetsPath))
            {
                Directory.CreateDirectory(streamingAssetsPath);
            }
            BuildAssetBundleOptions options = BuildAssetBundleOptions.StrictMode;
#if UNITY_EDITOR && !RELEASE
            options |= BuildAssetBundleOptions.UncompressedAssetBundle;
#else
            options |= BuildAssetBundleOptions.ChunkBasedCompression;
#endif
            var a = BuildPipeline.BuildAssetBundles(streamingAssetsPath, m_bundleBuildList.ToArray(), options, EditorUserBuildSettings.activeBuildTarget);
            EditorUtility.ClearProgressBar();
            Debug.Log("AssetBundle打包完毕");
        }

        private static void SetAssetImporter(string assetPath, string assetBundleName, string assetBundleVariant)
        {
            AssetImporter importer = AssetImporter.GetAtPath(assetPath);
            if (importer != null)
            {
                bool isChanged = false;
                importer.assetBundleName = CheckChangedAndGetValue(importer.assetBundleName, assetBundleName, ref isChanged);
                importer.assetBundleVariant = CheckChangedAndGetValue(importer.assetBundleVariant, assetBundleVariant, ref isChanged);
                if (isChanged)
                {
                    importer.SaveAndReimport();
                }
            }
        }

        private static void ResetAssetImporter(string assetPath)
        {
            AssetImporter importer = AssetImporter.GetAtPath(assetPath);
            if (importer != null)
            {
                bool isChanged = false;
                importer.assetBundleName = CheckChangedAndGetValue(importer.assetBundleName, "", ref isChanged);
                if (isChanged)
                {
                    importer.SaveAndReimport();
                }
            }
        }

        private static string GetAssetBundleName(string assetFolderPath)
        {
            return assetFolderPath.Substring(m_resFolderAssetPath.Length + 2).ToLower().Replace("/", "_");
        }

        private static T CheckChangedAndGetValue<T>(T origin, T target, ref bool isChanged)
        {
            if (!TypeMethodUtility.CheckEqual(origin, target))
            {
                isChanged = true;
            }
            return target;
        }

        private static AssetBundleInfoCollection CreateBundleInfoContainer()
        {
            string path = "Assets/Res/PreDownload/AssetBundleInfoCollection.asset";
            AssetBundleInfoCollection collection = AssetDatabase.LoadAssetAtPath<AssetBundleInfoCollection>(path);
            if (collection == null)
            {
                collection = new AssetBundleInfoCollection();
                AssetDatabase.CreateAsset(collection, path);
            }
            return collection;
        }

        private static string CreateMd5FromFile(string filePath)
        {
            var oMd5Hasher = new MD5CryptoServiceProvider();

            if (!File.Exists(filePath))
                return "";
            var oFileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            byte[] arrbytHashValue = oMd5Hasher.ComputeHash(oFileStream); //计算指定Stream 对象的哈希值
            oFileStream.Close();
            string strMd5 = BitConverter.ToString(arrbytHashValue).Replace("-", "");
            return strMd5;
        }
    }
}
#endif