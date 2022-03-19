
namespace MonMoose.Core
{
    public class StructHolder<T> : ClassPoolObj
        where T : struct
    {
        public T value;

        public override void OnRelease()
        {
            value = default(T);
        }
    }
}
