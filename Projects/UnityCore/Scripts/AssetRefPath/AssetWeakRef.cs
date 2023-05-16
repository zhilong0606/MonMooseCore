using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonMoose.Core
{
    [Serializable]
    public struct AssetWeakRef
    {
        public string path;
        public string guid;

        public AssetWeakRef(string path, string guid)
        {
            this.path = path;
            this.guid = guid;
        }
    }
}
