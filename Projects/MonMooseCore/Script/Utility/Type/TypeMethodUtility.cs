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
            MethodHolder<byte> byteMethodHolder = new MethodHolder<byte>();
            byteMethodHolder.Add = (a, b) => (byte) (a + b);
            byteMethodHolder.Minus = (a, b) => (byte) (a - b);
            byteMethodHolder.Multiply = (a, b) => (byte) (a * b);
            byteMethodHolder.Divide = (a, b) => (byte) (a / b);
            byteMethodHolder.CheckEqual = (a, b) => a == b;
            byteMethodHolder.TryParse = byte.TryParse;
            AddMethodHolder(byteMethodHolder);

            MethodHolder<sbyte> sbyteMethodHolder = new MethodHolder<sbyte>();
            sbyteMethodHolder.Add = (a, b) => (sbyte)(a + b);
            sbyteMethodHolder.Minus = (a, b) => (sbyte)(a - b);
            sbyteMethodHolder.Multiply = (a, b) => (sbyte)(a * b);
            sbyteMethodHolder.Divide = (a, b) => (sbyte)(a / b);
            sbyteMethodHolder.CheckEqual = (a, b) => a == b;
            sbyteMethodHolder.TryParse = sbyte.TryParse;
            AddMethodHolder(sbyteMethodHolder);

            MethodHolder<short> shortMethodHolder = new MethodHolder<short>();
            shortMethodHolder.Add = (a, b) => (short) (a + b);
            shortMethodHolder.Minus = (a, b) => (short) (a - b);
            shortMethodHolder.Multiply = (a, b) => (short) (a * b);
            shortMethodHolder.Divide = (a, b) => (short) (a / b);
            shortMethodHolder.CheckEqual = (a, b) => a == b;
            shortMethodHolder.TryParse = short.TryParse;
            AddMethodHolder(shortMethodHolder);

            MethodHolder<ushort> ushortMethodHolder = new MethodHolder<ushort>();
            ushortMethodHolder.Add = (a, b) => (ushort)(a + b);
            ushortMethodHolder.Minus = (a, b) => (ushort)(a - b);
            ushortMethodHolder.Multiply = (a, b) => (ushort)(a * b);
            ushortMethodHolder.Divide = (a, b) => (ushort)(a / b);
            ushortMethodHolder.CheckEqual = (a, b) => a == b;
            ushortMethodHolder.TryParse = ushort.TryParse;
            AddMethodHolder(ushortMethodHolder);

            MethodHolder<int> intMethodHolder = new MethodHolder<int>();
            intMethodHolder.Add = (a, b) => a + b;
            intMethodHolder.Minus = (a, b) => a - b;
            intMethodHolder.Multiply = (a, b) => a * b;
            intMethodHolder.Divide = (a, b) => a / b;
            intMethodHolder.CheckEqual = (a, b) => a == b;
            intMethodHolder.TryParse = int.TryParse;
            AddMethodHolder(intMethodHolder);

            MethodHolder<uint> uintMethodHolder = new MethodHolder<uint>();
            uintMethodHolder.Add = (a, b) => a + b;
            uintMethodHolder.Minus = (a, b) => a - b;
            uintMethodHolder.Multiply = (a, b) => a * b;
            uintMethodHolder.Divide = (a, b) => a / b;
            uintMethodHolder.CheckEqual = (a, b) => a == b;
            uintMethodHolder.TryParse = uint.TryParse;
            AddMethodHolder(uintMethodHolder);

            MethodHolder<long> longMethodHolder = new MethodHolder<long>();
            longMethodHolder.Add = (a, b) => a + b;
            longMethodHolder.Minus = (a, b) => a - b;
            longMethodHolder.Multiply = (a, b) => a * b;
            longMethodHolder.Divide = (a, b) => a / b;
            longMethodHolder.CheckEqual = (a, b) => a == b;
            longMethodHolder.TryParse = long.TryParse;
            AddMethodHolder(longMethodHolder);

            MethodHolder<ulong> ulongMethodHolder = new MethodHolder<ulong>();
            ulongMethodHolder.Add = (a, b) => a + b;
            ulongMethodHolder.Minus = (a, b) => a - b;
            ulongMethodHolder.Multiply = (a, b) => a * b;
            ulongMethodHolder.Divide = (a, b) => a / b;
            ulongMethodHolder.CheckEqual = (a, b) => a == b;
            ulongMethodHolder.TryParse = ulong.TryParse;
            AddMethodHolder(ulongMethodHolder);

            MethodHolder<float> floatMethodHolder = new MethodHolder<float>();
            floatMethodHolder.Add = (a, b) => a + b;
            floatMethodHolder.Minus = (a, b) => a - b;
            floatMethodHolder.Multiply = (a, b) => a * b;
            floatMethodHolder.Divide = (a, b) => a / b;
            floatMethodHolder.CheckEqual = (a, b) => Math.Abs(a - b) < 1e-4;
            floatMethodHolder.TryParse = float.TryParse;
            AddMethodHolder(floatMethodHolder);

            MethodHolder<double> doubleMethodHolder = new MethodHolder<double>();
            doubleMethodHolder.Add = (a, b) => a + b;
            doubleMethodHolder.Minus = (a, b) => a - b;
            doubleMethodHolder.Multiply = (a, b) => a * b;
            doubleMethodHolder.Divide = (a, b) => a / b;
            doubleMethodHolder.CheckEqual = (a, b) => Math.Abs(a - b) < 1e-4;
            doubleMethodHolder.TryParse = double.TryParse;
            AddMethodHolder(doubleMethodHolder);

            MethodHolder<bool> boolMethodHolder = new MethodHolder<bool>();
            boolMethodHolder.CheckEqual = (a, b) => a == b;
            boolMethodHolder.TryParse = bool.TryParse;
            AddMethodHolder(boolMethodHolder);

            MethodHolder<string> stringMethodHolder = new MethodHolder<string>();
            stringMethodHolder.CheckEqual = (a, b) => a == b;
            stringMethodHolder.TryParse = (string a, out string b) =>
            {
                b = a;
                return true;
            };
            AddMethodHolder(stringMethodHolder);
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
