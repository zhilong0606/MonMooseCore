using System;
using System.Collections.Generic;

namespace MonMooseCore
{
    public class ListenedValue
    {
        protected static Dictionary<Type, Delegate> m_equalFuncMap = new Dictionary<Type, Delegate>()
        {
            {typeof(int), (ListenedValue<int>.DelegateEqualFunc)EqualInt},
            {typeof(bool), (ListenedValue<bool>.DelegateEqualFunc)EqualBool},
            {typeof(float), (ListenedValue<float>.DelegateEqualFunc)EqualFloat},
            {typeof(double), (ListenedValue<double>.DelegateEqualFunc)EqualDouble},
        };

        public static void RegisterEqualFunc<T>(ListenedValue<T>.DelegateEqualFunc func)
        {
            Type type = typeof(T);
            if (!m_equalFuncMap.ContainsKey(type))
            {
                m_equalFuncMap.Add(type, func);
            }
        }

        private static bool EqualInt(int i1, int i2)
        {
            return i1 == i2;
        }

        private static bool EqualBool(bool b1, bool b2)
        {
            return b1 == b2;
        }

        private static bool EqualFloat(float f1, float f2)
        {
            return f1.IsEqualTo(f2);
        }

        private static bool EqualDouble(double d1, double d2)
        {
            return Math.Abs(d1 - d2) <= double.Epsilon;
        }
    }

    public class ListenedValue<T> : ListenedValue
    {
        public delegate void DelegateValueChanged(T pre, T cur);
        public delegate bool DelegateEqualFunc(T pre, T cur);

        private T m_preValue;
        private T m_curValue;
        private Func<T, T, bool> m_equalFunc = EqualDefault;

        private bool m_isValueChanged = false;

        public DelegateValueChanged actionOnValueChanged;

        public T curValue
        {
            get { return m_curValue; }
            set
            {
                SetValueSilently(value);
                ApplyChange();
            }
        }

        public T preValue
        {
            get { return m_preValue; }
        }

        public bool isValueChanged
        {
            get { return m_isValueChanged; }
        }

        public ListenedValue()
            : this(default(T))
        {
        }

        public ListenedValue(T value)
        {
            m_curValue = value;
            m_preValue = value;
            Initialize();
        }

        private void Initialize()
        {
            Type t = typeof(T);
            if (m_equalFuncMap.ContainsKey(t))
            {
                m_equalFunc = (Func<T, T, bool>)m_equalFuncMap[t];
            }
        }

        public void SetValueSilently(T value)
        {
            if (!m_equalFunc(m_curValue, value))
            {
                m_preValue = m_curValue;
                m_curValue = value;
                m_isValueChanged = true;
            }
        }

        public void ApplyChange()
        {
            if (m_isValueChanged)
            {
                m_isValueChanged = false;
                if (actionOnValueChanged != null)
                {
                    actionOnValueChanged(m_preValue, m_curValue);
                }
            }
        }

        private static bool EqualDefault(T o1, T o2)
        {
            throw new NotImplementedException();
        }
    }
}