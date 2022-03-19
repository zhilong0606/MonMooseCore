using System;

namespace MonMoose.Core
{
    public class SinLerpFunc : ILerpFunc
    {
        public float GetValue(float f)
        {
            return (float)Math.Sin(f / 2f * Math.PI);
        }
    }
}
