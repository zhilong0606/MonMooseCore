namespace MonMoose.Core
{
    public class SquareLerpFunc : ILerpFunc
    {
        public float GetValue(float f)
        {
            return f * f;
        }
    }
}