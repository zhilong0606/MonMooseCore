using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MonMoose.Core
{
    [Serializable]
    public class UIBindItemInfo
    {
        public string nameStr;
        [OnValueChanged("OnBindObjChanged")]
        public GameObject bindObj;
        public EnumString<UIBindItemType> bindType;
        public string remarkStr;

        private void OnBindObjChanged()
        {
            UIBindItemType curBindType = bindType.value;
            if (curBindType == UIBindItemType.None)
            {
                UIBindItemType matchedBindType = UIBindUtility.GetMatchedBindType(bindObj);
                bindType.value = matchedBindType;
            }
            if (string.IsNullOrEmpty(nameStr))
            {
                nameStr = bindObj.name;
            }
        }
    }
}
