using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonMoose.Core;

namespace MonMoose.GameLogic
{
    [CreateAssetMenu(fileName = "ParticleDataInventory", menuName = "Custom Asset/ParticleDataInventory")]
    public class ParticleDataInventory : ScriptableObject
    {
        public List<ParticleData> particleList = new List<ParticleData>();
    }
}
