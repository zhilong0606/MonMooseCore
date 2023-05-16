using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonMoose.Core
{
    [Serializable]
    public struct SerializedElement
    {
        [SerializeField] public TypeInfo typeInfo;

        [SerializeField] public string jsonData;
    }
}