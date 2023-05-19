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
            float newUnscaledDeltaTime = unscaledDeltaTime + addTime;
            deltaTime = deltaTime * newUnscaledDeltaTime / unscaledDeltaTime;
        }

        public void ResizeByDeltaTime(float time)
        {
            float newDeltaTime = time;
            unscaledDeltaTime = unscaledDeltaTime * newDeltaTime / deltaTime;
        }

        public void ResizeByUnscaledDeltaTime(float time)
        {
            float newUnscaledDeltaTime = time;
            deltaTime = deltaTime * newUnscaledDeltaTime / unscaledDeltaTime;
        }
    }
}
