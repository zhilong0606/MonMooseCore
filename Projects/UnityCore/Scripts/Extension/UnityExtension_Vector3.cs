using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonMoose.Core
{
    public static partial class UnityExtension
    {
        public static Quaternion LookRotation(this Vector3 forward)
        {
            if (forward.sqrMagnitude < float.Epsilon)
            {
                forward = Vector3.forward;
            }
            return Quaternion.LookRotation(forward);
        }
    }
}
