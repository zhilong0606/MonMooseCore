using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonMoose.Core
{
    [CreateAssetMenu(fileName = "AssetBundleInfoCollection", menuName = "Custom Asset/AssetBundleInfoCollection")]
    public class AssetBundleInfoCollection : ScriptableObject
    {
        public List<AssetBundleInfo> infoList = new List<AssetBundleInfo>();
    }
}
