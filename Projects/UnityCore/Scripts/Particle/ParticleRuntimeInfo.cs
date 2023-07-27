using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonMoose.Core;

namespace MonMoose.GameLogic
{
    public class ParticleRuntimeInfo : ClassPoolObj
    {
        public GameObject go;
        public ParticleConfig config;
        public Action actionOnEnd;
        public float m_curTime;

        public bool isOver
        {
            get { return m_curTime >= config.lifeTime; }
        }

        public override void OnRelease()
        {
            base.OnRelease();
            go = null;
            m_curTime = default;
        }

        public void Tick(TimeSlice timeSlice)
        {
            if (config.isLoop)
            {
                return;
            }
            m_curTime += timeSlice.deltaTime;
        }
    }
}
