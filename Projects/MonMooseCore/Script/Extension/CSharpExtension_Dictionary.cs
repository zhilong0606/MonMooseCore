using System.Collections.Generic;

namespace MonMooseCore
{
    public static partial class CSharpExtension
    {
        private static readonly DictionaryReleaser<int> intDicReleaser = new DictionaryReleaser<int>();

        public static void ClearAll<U>(this Dictionary<int, U> map) where U : class
        {
            intDicReleaser.Clear(map);
        }

        public static U GetClassValue<T, U>(this Dictionary<T, U> map, T key) where U : class
        {
            U value;
            map.TryGetValue(key, out value);
            return value;
        }

        public static U? GetStructValue<T, U>(this Dictionary<T, U> map, T key) where U : struct
        {
            U value;
            if (map.TryGetValue(key, out value))
            {
                return value;
            }
            return null;
        }

        private static bool TryGetKey<T, U>(this Dictionary<T, U> map, U value, out T key)
        {
            bool findKey = false;
            key = default(T);
            Dictionary<T, U>.Enumerator enumerator = map.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.Value.Equals(value))
                {
                    key = enumerator.Current.Key;
                    findKey = true;
                    break;
                }
            }
            enumerator.Dispose();
            return findKey;
        }

        public static void RemoveValue<T, U>(this Dictionary<T, U> map, U value)
        {
            T key;
            if (map.TryGetKey(value, out key))
            {
                map.Remove(key);
            }
        }
        private class DictionaryReleaser<T>
        {
            private List<T> objList = new List<T>();

            public void Clear<U>(Dictionary<T, U> map) where U : class
            {
                Dictionary<T, U>.Enumerator enumerator = map.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    objList.Add(enumerator.Current.Key);
                }
                enumerator.Dispose();
                for (int i = 0; i < objList.Count; ++i)
                {
                    map[objList[i]] = null;
                }
                objList.Clear();
                map.Clear();
            }
        }
    }
}
