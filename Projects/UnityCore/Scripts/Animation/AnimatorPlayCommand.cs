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
        public Action actionOnEnd;

        //actionOnEnd:����һ�������
        public static AnimatorPlayCommand Create(AnimatorStateType stateType, string name, float fadeTime, Action actionOnEnd = null)
        {
            AnimatorPlayCommand obj = new AnimatorPlayCommand();
            obj.stateType = stateType;
            obj.name = name;
            obj.fadeTime = fadeTime;
            obj.actionOnEnd = actionOnEnd;
            return obj;
        }
    }
}
