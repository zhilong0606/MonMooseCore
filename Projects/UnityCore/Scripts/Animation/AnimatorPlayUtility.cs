using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonMoose.Core
{
    public static class AnimatorPlayUtility
    {
        public static bool CheckLoop(AnimatorStateType stateType)
        {
            return stateType == AnimatorStateType.Loop;
        }
    }
}
