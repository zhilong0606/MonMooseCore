using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonMoose.Core
{
    public class UIBindItemTypeInfo
    {
        public UIBindItemType bindType;
        public Type classType;

        public UIBindItemTypeInfo(UIBindItemType bindType, Type classType)
        {
            this.bindType = bindType;
            this.classType = classType;
        }
    }
}
