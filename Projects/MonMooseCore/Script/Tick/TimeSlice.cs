using System;
using System.Collections;
using System.Collections.Generic;

namespace MonMoose.Core
{
    public struct TimeSlice
    {
        public float deltaTime;
        public float unscaledDeltaTime;

        public void AddDeltaTime(float addTime)
        {
            float newDeltaTime = deltaTime + addTime;
            unscaledDeltaTime = unscaledDeltaTime * newDeltaTime / deltaTime;
        }

        public void AddUnscaledDeltaTime(float addTime)
        {
            float newUnScaledDeltaTime = unscaledDeltaTime + addTime;
            deltaTime = deltaTime * newUnScaledDeltaTime / unscaledDeltaTime;
        }
    }
}
