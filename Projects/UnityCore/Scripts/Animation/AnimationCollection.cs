using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonMoose.Core
{
    [CreateAssetMenu(fileName = "AnimationCollection", menuName = "Custom Asset/AnimationCollection")]
    public class AnimationCollection : ScriptableObject
    {
        public List<AnimationClip> clipList = new List<AnimationClip>();
    }
}
