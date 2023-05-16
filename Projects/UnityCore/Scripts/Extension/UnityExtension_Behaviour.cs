using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonMoose.Core
{
    public static partial class UnityExtension
    {
        public static void SetEnabledSafely(this Behaviour compo, bool enabled)
        {
            if (compo != null)
            {
                compo.enabled = enabled;
            }
        }
    }
}
