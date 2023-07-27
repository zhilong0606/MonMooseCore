using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MonMoose.Core.AnimatorPlayController;

namespace MonMoose.Core
{
    public struct AnimatorPlayCommand
    {
        public int layerIndex;
        public AnimatorStateType stateType;
        public float fadeTime;
        public string name;
        public Action<bool> actionOnEnd;

        public static AnimatorPlayCommand Create(AnimatorStateType stateType, string name, float fadeTime, Action<bool> actionOnEnd)
        {
            AnimatorPlayCommand obj = new AnimatorPlayCommand();
            obj.stateType = stateType;
            obj.name = name;
            obj.fadeTime = fadeTime;
            obj.actionOnEnd = actionOnEnd;
            return obj;
        }

        public static AnimatorPlayCommand Create(AnimatorStateType stateType, string name, float fadeTime)
        {
            AnimatorPlayCommand obj = new AnimatorPlayCommand();
            obj.stateType = stateType;
            obj.name = name;
            obj.fadeTime = fadeTime;
            obj.actionOnEnd = null;
            return obj;
        }

        public void InvokeEndIfAlwaysCall(bool interrupt)
        {
            Action<bool> temp = actionOnEnd;
            actionOnEnd = null;
            temp.InvokeSafely(interrupt);
        }
    }
}
