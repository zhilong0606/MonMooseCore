using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonMoose.Core
{
    [Serializable]
    public class EnumString<T> : EnumString
        where T : struct, Enum
    {
        public T value;

        public EnumString() : this(default)
        {
        }

        public EnumString(T value)
        {
            this.value = value;
        }

        public bool CheckValueEquals(T other)
        {
            return value.GetHashCode() == other.GetHashCode();
        }

        public override void OnBeforeSerialize()
        {
            m_enumName = value.ToString();
        }

        public override void OnAfterDeserialize()
        {
            T enumValue;
            if (Enum.TryParse(m_enumName, out enumValue))
            {
                value = enumValue;
            }
        }

        public static bool operator ==(EnumString<T> p1, EnumString<T> p2)
        {
            return p1.Equals(p2);
        }


        public static bool operator !=(EnumString<T> p1, EnumString<T> p2)
        {
            return !p1.Equals(p2);
        }

        public static bool operator ==(EnumString<T> p1, T p2)
        {
            return p1.CheckValueEquals(p2);
        }


        public static bool operator !=(EnumString<T> p1, T p2)
        {
            return !p1.CheckValueEquals(p2);
        }

        public static implicit operator T(EnumString<T> enumString)
        {
            return enumString.value;
        }

        public override bool Equals(object obj)
        {
            EnumString<T> enumString = obj as EnumString<T>;
            if(enumString != null)
            {
                return CheckValueEquals(obj as EnumString<T>);
            }
            return base.Equals(obj);
        }

        protected bool Equals(EnumString<T> other)
        {
            return CheckValueEquals(other.value);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return value.GetHashCode();
            }
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }

    [Serializable]
    public abstract class EnumString : ISerializationCallbackReceiver
    {
        [SerializeField]
        protected string m_enumName;

        public abstract void OnBeforeSerialize();
        public abstract void OnAfterDeserialize();
    }
}