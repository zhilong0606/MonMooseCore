using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonMoose.Core
{
    public class ResourceLoaderAssetBundle : ResourceLoader
    {
        private Dictionary<string, string> m_assetPathToBundlePathMap = new Dictionary<string, string>();

        public override bool isInitialized
        {
            get { return true; }
        }

        protected override void OnInit()
        {
            base.OnInit();
            AssetBundle containerBundle = AssetBundle.LoadFromFile("Assets/StreamingAssets/predownload.ab");
            AssetBundleInfoCollection collection = containerBundle.LoadAsset<AssetBundleInfoCollection>("Assets/Res/PreDownload/AssetBundleInfoCollection.asset");
            AssetBundle manifestBundle = AssetBundle.LoadFromFile("Assets/StreamingAssets/StreamingAssets");
            var assets = manifestBundle.LoadAllAssets();
            AssetBundleManifest manifest = null;
            foreach (UnityEngine.Object asset in assets)
            {
                manifest = asset as AssetBundleManifest;
                if (manifest != null)
                {
                    break;
                }
            }
            if (manifest != null)
            {

            }
            foreach (var bundleInfo in collection.infoList)
            {
                foreach (var assetPath in bundleInfo.assetPathList)
                {
                    m_assetPathToBundlePathMap.Add(assetPath, bundleInfo.bundlePath);
                }
            }
        }

        protected override T OnLoadSync<T>(string path, string resourceSubName)
        {
            string bundlePath;
            if (m_assetPathToBundlePathMap.TryGetValue(path, out bundlePath))
            {
                AssetBundle bundle = AssetBundle.LoadFromFile(bundlePath);
                if (!string.IsNullOrEmpty(resourceSubName))
                {
                    UnityEngine.Object[] resourceObjs = bundle.LoadAssetWithSubAssets(path);
                    int count = resourceObjs.Length;
                    for (int i = 0; i < count; ++i)
                    {
                        T resourceObj = resourceObjs[i] as T;
                        if (resourceObj != null && resourceObj.name == resourceSubName)
                        {
                            return resourceObj;
                        }
                    }
                }
                else
                {
                    return bundle.LoadAsset<T>(path);
                }
            }
            return null;
        }

        protected override ResourceLoadTask CreateLoadTask<T>()
        {
            return ClassPoolManager.instance.Fetch<ResourceLoadTaskAssetBundle<T>>();
        }
    }
}
