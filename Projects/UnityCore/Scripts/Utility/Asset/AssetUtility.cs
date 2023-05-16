using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonMoose.Core
{
    public static class AssetUtility
    {
        public static GameObject GetPrefab(string path)
        {
            return ResourceManager.instance.LoadSync<GameObject>(path);
        }

        public static Sprite GetSprite(string path)
        {
            return ResourceManager.instance.LoadSync<Sprite>(path);
        }
    }
}
