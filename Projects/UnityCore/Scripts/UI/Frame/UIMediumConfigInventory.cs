using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonMoose.Core
{
    [CreateAssetMenu(fileName = "UIMediumConfigInventory", menuName = "Custom Asset/UI/UIMediumConfigInventory")]
    public class UIMediumConfigInventory : ScriptableObject
    {
        public const string path = "Assets/Res/UI/Common/Medium/UIMediumConfigInventory.asset";

        public List<UIMediumConfig> configList = new List<UIMediumConfig>();

        public UIMediumConfig GetConfig(UIMediumId id)
        {
            for (int i = 0; i < configList.Count; ++i)
            {
                if (configList[i].mediumId.value == id)
                {
                    return configList[i];
                }
            }
            return null;
        }
    }
}
