using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonMoose.Core
{
    [Serializable]
    public struct TypeInfo
    {
        [SerializeField] public string fullName;
        [SerializeField] public string assemblyName;
    }
}