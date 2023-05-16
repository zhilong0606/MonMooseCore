using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace MonMoose.Core
{
	public static class ReflectionUtility
    {
        public static void SetNonPublicField(object obj, string fieldName, object value)
        {
            FieldInfo fieldInfo = GetNonPublicFieldInfo(obj, fieldName);
            if (fieldInfo != null)
            {
                fieldInfo.SetValue(obj, value);
            }
        }

        public static object GetNonPublicField(object obj, string fieldName)
        {
            FieldInfo fieldInfo = GetNonPublicFieldInfo(obj, fieldName);
            if(fieldInfo != null)
            {
                return fieldInfo.GetValue(obj);
            }
            return null;
        }

        public static T GetNonPublicField<T>(object obj, string fieldName)
        {
            object value = GetNonPublicField(obj, fieldName);
            if (value != null)
            {
                return (T)value;
            }
            return default;
        }

        public static FieldInfo GetNonPublicFieldInfo(object obj, string fieldName)
        {
            return obj.GetType().GetField(fieldName);
        }
	}
}
