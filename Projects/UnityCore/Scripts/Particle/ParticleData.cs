using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonMoose.Core;

namespace MonMoose.GameLogic
{
    [Serializable]
    public class ParticleData
    {
        public int id;
        public string desc;
        public AssetWeakRef effect;
    }
}
