using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace MonMoose.Core
{
    public static partial class CSharpExtension
    {
        public static T GetValueSafely<T>(this IEnumerable<T> list, int index, T defaultValue = default(T))
        {
            if(index < 0)
            {
                return defaultValue;
            }
            int i = 0;
            foreach (var item in list)
            {
                if(i == index)
                {
                    return item;
                }
                i++;
            }
            return defaultValue;
        }
    }
}
