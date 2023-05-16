using UnityEngine;

namespace MonMoose.Core
{
    [CreateAssetMenu(fileName = "CurveLerpFunc", menuName = "Custom Asset/CurveLerpFunc")]
    public class CurveLerpFunc : ScriptableObject, ILerpFunc
    {
        public AnimationCurve m_curve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

        public float GetValue(float f)
        {
            return m_curve.Evaluate(f);
        }
    }
}
