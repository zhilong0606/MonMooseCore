using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonMoose.Core
{
    [Serializable]
    public class UIBindItemInfo
    {
        public string nameStr;
        public GameObject bindObj;
        public EnumString<UIBindItemType> bindType;
        public string remarkStr;
    }
}
