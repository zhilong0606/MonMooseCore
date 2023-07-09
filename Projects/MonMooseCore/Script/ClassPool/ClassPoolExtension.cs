using System.Collections;
using System.Collections.Generic;

namespace MonMoose.Core
{
    public static class ClassPoolExtension
    {
        public static void Release(this object obj)
        {
            ClassPoolManager.instance.Release(obj);
        }

        public static void ReleaseAll(this IList objList)
        {
            ClassPoolManager.instance.ReleaseAll(objList);
        }

        public static void ReleaseAll<T>(this T[] objs)
        {
            ClassPoolManager.instance.ReleaseAll(objs);
        }
    }
}
