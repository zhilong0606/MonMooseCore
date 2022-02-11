using System;

namespace MonMooseCore.Data
{

    public abstract class BasicDataValue<T> : BasicDataValue where T : struct
    {
        public T value;

        public override void Init(string str)
        {
            value = AnalyzeValue(str);
        }

        protected virtual T AnalyzeValue(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return default(T);
            }
            T analyzedValue;
            if (TypeMethodUtility.TryParse(str, out analyzedValue))
            {
                return analyzedValue;
            }
            throw new Exception();
        }
    }

    public abstract class BasicDataValue : DataValue
    {
        public abstract void Init(string str);
    }
}
