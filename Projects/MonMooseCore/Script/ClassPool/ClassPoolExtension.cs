using System.Collections.Generic;

namespace MonMooseCore
{
    public static class ClassPoolExtension
    {
        public static void Release(this IClassPoolObj obj)
        {
            if (obj != null && obj.creator != null)
            {
                obj.creator.Release(obj);
            }
        }

        public static void ReleaseAll<T>(this IList<T> objList) where T : IClassPoolObj
        {
            if (objList == null)
            {
                return;
            }
            int count = objList.Count;
            for (int i = 0; i < count; ++i)
            {
                objList[i].Release();
            }
            objList.Clear();
        }

        public static void ReleaseAll<T>(this T[] objs) where T : IClassPoolObj
        {
            if (objs == null)
            {
                return;
            }
            int count = objs.Length;
            for (int i = 0; i < count; ++i)
            {
                objs[i].Release();
                objs[i] = default(T);
            }
        }
    }
}
