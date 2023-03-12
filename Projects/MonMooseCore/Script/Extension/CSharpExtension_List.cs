using System.Collections;
using System.Collections.Generic;

namespace MonMoose.Core
{
    public static partial class CSharpExtension
    {
        public static bool TryGetValue<T>(this IList<T> l, int index, out T value)
        {
            if (index >= 0 && index < l.Count)
            {
                value = l[index];
                return true;
            }
            value = default(T);
            return false;
        }

        public static void AddNotContains<T>(this IList<T> l, T value)
        {
            if (!l.Contains(value))
            {
                l.Add(value);
            }
        }

        public static void AddNotContainsNotNull<T>(this IList<T> l, T value)
        {
            if (value == null)
            {
                return;
            }
            AddNotContains(l, value);
        }

        public static void AddNotNull<T>(this IList<T> l, T value) where T : class
        {
            if (value == null)
            {
                return;
            }
            if (!l.Contains(value))
            {
                l.Add(value);
            }
        }

        public static bool CheckIndex(this IList list, int index)
        {
            return list != null && index >= 0 && index < list.Count;
        }

        public static T GetValueSafely<T>(this IList<T> list, int index, T defaultValue = default(T))
        {
            if (list != null && index >= 0 && index < list.Count)
            {
                return list[index];
            }
            return defaultValue;
        }

        public static T GetValueSafelyByLast<T>(this IList<T> list, int index, T defaultValue = default(T))
        {
            if (list != null && list.Count > 0)
            {
                return GetValueSafely(list, index, list[list.Count - 1]);
            }
            return defaultValue;
        }
    }
}
