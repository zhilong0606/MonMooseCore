using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonMoose.Core
{
    [Serializable]
    public class AssetBundleInfo
    {
        public string bundleName = string.Empty;
        public string bundlePath = string.Empty;
        public List<string> assetPathList = new List<string>();
    }
}
