namespace MonMoose.Core
{
    public class StaticLerpFloat : AbstractStaticLerp<float>
    {
        protected override float Lerp(float start, float end, float f)
        {
            return start + (end - start) * f;
        }
    }
}