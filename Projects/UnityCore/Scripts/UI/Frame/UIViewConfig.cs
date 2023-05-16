using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonMoose.Core
{
    [Serializable]
    public class UIViewConfig
    {
        public string nameStr;
        public string typeName;
        public AssetWeakRef prefabWeakRef;

        [NonSerialized]
        public Type classType;
    }
}
