using System;
using System.Collections.Generic;

namespace MonMooseCore
{
    public static class TypeMethodUtility
    {
        private delegate bool TryParseDelegate<T>(string str, out T value);
        private delegate T BinocularOperationDelegate<T>(T a, T b);
        private delegate bool CheckEqualDelegate<T>(T a, T b);

        private class MethodHolder
        {
            public Type type;
        }

        private class MethodHolder<T> : MethodHolder
        {
            public MethodHolder()
            {
                type = typeof(T);
            }

            public BinocularOperationDelegate<T> Add;
            public BinocularOperationDelegate<T> Minus;
            public BinocularOperationDelegate<T> Multiply;
            public BinocularOperationDelegate<T> Divide;
            public CheckEqualDelegate<T> CheckEqual;
            public TryParseDelegate<T> TryParse;
        }

        private static List<MethodHolder> m_valueTypeMethodHolderList = new List<MethodHolder>();

        static TypeMethodUtility()
        {
            AddMethodHolder(new MethodHolder<bool>()
            {
                CheckEqual = (a, b) => a == b,
                TryParse = bool.TryParse,
            });

            AddMethodHolder(new MethodHolder<byte>()
            {
                Add = (a, b) => (byte)(a + b),
                Minus = (a, b) => (byte)(a - b),
                Multiply = (a, b) => (byte)(a * b),
                Divide = (a, b) => (byte)(a / b),
                CheckEqual = (a, b) => a == b,
                TryParse = byte.TryParse,
            });

            AddMethodHolder(new MethodHolder<sbyte>()
            {
                Add = (a, b) => (sbyte)(a + b),
                Minus = (a, b) => (sbyte)(a - b),
                Multiply = (a, b) => (sbyte)(a * b),
                Divide = (a, b) => (sbyte)(a / b),
                CheckEqual = (a, b) => a == b,
                TryParse = sbyte.TryParse,
            });

            AddMethodHolder(new MethodHolder<short>()
            {
                Add = (a, b) => (short)(a + b),
                Minus = (a, b) => (short)(a - b),
                Multiply = (a, b) => (short)(a * b),
                Divide = (a, b) => (short)(a / b),
                CheckEqual = (a, b) => a == b,
                TryParse = short.TryParse,
            });

            AddMethodHolder(new MethodHolder<ushort>()
            {
                Add = (a, b) => (ushort)(a + b),
                Minus = (a, b) => (ushort)(a - b),
                Multiply = (a, b) => (ushort)(a * b),
                Divide = (a, b) => (ushort)(a / b),
                CheckEqual = (a, b) => a == b,
                TryParse = ushort.TryParse,
            });

            AddMethodHolder(new MethodHolder<int>()
            {
                Add = (a, b) => (int)(a + b),
                Minus = (a, b) => (int)(a - b),
                Multiply = (a, b) => (int)(a * b),
                Divide = (a, b) => (int)(a / b),
                CheckEqual = (a, b) => a == b,
                TryParse = int.TryParse,
            });

            AddMethodHolder(new MethodHolder<uint>()
            {
                Add = (a, b) => (uint)(a + b),
                Minus = (a, b) => (uint)(a - b),
                Multiply = (a, b) => (uint)(a * b),
                Divide = (a, b) => (uint)(a / b),
                CheckEqual = (a, b) => a == b,
                TryParse = uint.TryParse,
            });

            AddMethodHolder(new MethodHolder<long>()
            {
                Add = (a, b) => (long)(a + b),
                Minus = (a, b) => (long)(a - b),
                Multiply = (a, b) => (long)(a * b),
                Divide = (a, b) => (long)(a / b),
                CheckEqual = (a, b) => a == b,
                TryParse = long.TryParse,
            });

            AddMethodHolder(new MethodHolder<ulong>()
            {
                Add = (a, b) => (ulong)(a + b),
                Minus = (a, b) => (ulong)(a - b),
                Multiply = (a, b) => (ulong)(a * b),
                Divide = (a, b) => (ulong)(a / b),
                CheckEqual = (a, b) => a == b,
                TryParse = ulong.TryParse,
            });

            AddMethodHolder(new MethodHolder<float>()
            {
                Add = (a, b) => (float)(a + b),
                Minus = (a, b) => (float)(a - b),
                Multiply = (a, b) => (float)(a * b),
                Divide = (a, b) => (float)(a / b),
                CheckEqual = (a, b) => Math.Abs(a - b) < 1e-4,
                TryParse = float.TryParse,
            });

            AddMethodHolder(new MethodHolder<double>()
            {
                Add = (a, b) => (double)(a + b),
                Minus = (a, b) => (double)(a - b),
                Multiply = (a, b) => (double)(a * b),
                Divide = (a, b) => (double)(a / b),
                CheckEqual = (a, b) => Math.Abs(a - b) < 1e-4,
                TryParse = double.TryParse,
            });

            AddMethodHolder(new MethodHolder<string>()
            {
                CheckEqual = (a, b) => a == b,
                TryParse = (string a, out string b ) => { b = a; return true; },
            });
        }

        public static T Add<T>(T a, T b)
        {
            Type type = typeof(T);
            MethodHolder<T> holder = GetMethodHolder<T>(type);
            if (holder != null)
            {
                if (holder.Add != null)
                {
                    return holder.Add(a, b);
                }
            }
            DebugUtility.LogError("Not Match Add Method");
            return default(T);
        }

        public static T Minus<T>(T a, T b)
        {
            Type type = typeof(T);
            MethodHolder<T> holder = GetMethodHolder<T>(type);
            if (holder != null)
            {
                if (holder.Minus != null)
                {
                    return holder.Minus(a, b);
                }
            }
            DebugUtility.LogError("Not Match Minus Method");
            return default(T);
        }

        public static T Multiply<T>(T a, T b)
        {
            Type type = typeof(T);
            MethodHolder<T> holder = GetMethodHolder<T>(type);
            if (holder != null)
            {
                if (holder.Multiply != null)
                {
                    return holder.Multiply(a, b);
                }
            }
            DebugUtility.LogError("Not Match Multiply Method");
            return default(T);
        }

        public static T Divide<T>(T a, T b)
        {
            Type type = typeof(T);
            MethodHolder<T> holder = GetMethodHolder<T>(type);
            if (holder != null)
            {
                if (holder.Divide != null)
                {
                    return holder.Divide(a, b);
                }
            }
            DebugUtility.LogError("Not Match Divide Method");
            return default(T);
        }

        public static bool CheckEqual<T>(T a, T b)
        {
            Type type = typeof(T);
            MethodHolder<T> holder = GetMethodHolder<T>(type);
            if (holder != null)
            {
                if (holder.CheckEqual != null)
                {
                    return holder.CheckEqual(a, b);
                }
            }
            if (type.IsClass)
            {
                return Equals(a, b);
            }
            DebugUtility.LogError("Not Match CheckEqual Method");
            return false;
        }

        public static bool TryParse<T>(string str, out T value)
        {
            Type type = typeof(T);
            MethodHolder<T> holder = GetMethodHolder<T>(type);
            if (holder != null)
            {
                if (holder.TryParse != null)
                {
                    return holder.TryParse(str, out value);
                }
            }
            value = default(T);
            return false;
        }

        private static void AddMethodHolder(MethodHolder hodler)
        {
            m_valueTypeMethodHolderList.Add(hodler);
        }

        private static MethodHolder<T> GetMethodHolder<T>(Type type)
        {
            return GetMethodHolder(type) as MethodHolder<T>;
        }

        private static MethodHolder GetMethodHolder(Type type)
        {
            int count = m_valueTypeMethodHolderList.Count;
            for (int i = 0; i < count; ++i)
            {
                MethodHolder holder = m_valueTypeMethodHolderList[i];
                if (holder.type == type)
                {
                    return holder;
                }
            }
            return null;
        }
    }
}
