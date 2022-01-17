using System;

namespace MonMooseCore
{
    public class SinLerpFunc : ILerpFunc
    {
        public float GetValue(float f)
        {
            return (float)Math.Sin(f / 2f * Math.PI);
        }
    }
}
