using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonMoose.Core
{
    [CreateAssetMenu(fileName = "UIMediumConfig", menuName = "Custom Asset/UI/UIMediumConfig")]
    public class UIMediumConfig : ScriptableObject
    {
        public EnumString<UIMediumId> mediumId;
        public List<EnumString<UIMediumTagId>> tagIdList = new List<EnumString<UIMediumTagId>>();
        public bool stackOptimizable = true;
        public string typeName;
        public AssetWeakRef scriptWeakRef;
        public List<UIViewConfig> viewConfigList = new List<UIViewConfig>();

        [NonSerialized]
        public Type classType;

        public UIViewConfig GetViewConfigByName(string name)
        {
            for (int i = 0; i < viewConfigList.Count; ++i)
            {
                UIViewConfig viewConfig = viewConfigList[i];
                if (viewConfig != null && viewConfig.nameStr == name)
                {
                    return viewConfig;
                }
            }
            return null;
        }
    }
}
