using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonMoose.Core
{
    public class UIBindGroup : MonoBehaviour
    {
        public AssetWeakRef scriptWeakRef;
        public List<UIBindItemInfo> bindItemList = new List<UIBindItemInfo>();

        public UIBindItemInfo GetItemInfoByName(string nameStr)
        {
            for (int i = 0; i < bindItemList.Count; ++i)
            {
                if (bindItemList[i].nameStr == nameStr)
                {
                    return bindItemList[i];
                }
            }
            return null;
        }
    }
}
