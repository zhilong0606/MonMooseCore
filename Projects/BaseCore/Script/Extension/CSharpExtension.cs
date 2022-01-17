using System;

namespace MonMooseCore
{
    public static partial class CSharpExtension
    {
        public static bool IsZero(this float value)
        {
            return Math.Abs(value) < 1e-4;
        }

        public static bool IsEqualTo(this float v1, float v2)
        {
            return Math.Abs(v1 - v2) < 1e-4;
        }

        public static bool ToBool(this int value)
        {
            return value != 0;
        }
        public static bool ToBool(this sbyte value)
        {
            return value != 0;
        }

        public static int ToInt(this bool value)
        {
            return value ? 1 : 0;
        }

        public static float WithSign(this float v, bool isPositive)
        {
            float ret = Math.Abs(v);
            if (!isPositive)
            {
                ret *= -1f;
            }
            return ret;
        }

        public static bool Contains(this string str, char ch)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }
            int count = str.Length;
            for (int i = 0; i < count; ++i)
            {
                if (str[i] == ch)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
