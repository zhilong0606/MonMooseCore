using UnityEngine;

namespace MonMoose.Core
{
    public class StaticLerpVec3 : AbstractStaticLerp<Vector3>
    {
        protected override Vector3 Lerp(Vector3 start, Vector3 end, float f)
        {
            return start + (end - start) * f;
        }
    }
}