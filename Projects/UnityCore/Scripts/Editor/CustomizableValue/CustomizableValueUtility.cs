using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace MonMoose.Core
{
	public static class CustomizableValueUtility
	{
        public static void InitAllCustomizableValue(object obj)
        {
            List<CustomizableValue> list = GetAllCustomizableValue(obj);
            int maxId = 0;
            for (int i = 0; i < list.Count; ++i)
            {
                CustomizableValue customizableValue = list[i];
                if (customizableValue.customId > 0 && customizableValue.customId > maxId)
                {
                    maxId = customizableValue.customId;
                }
            }
            for (int i = 0; i < list.Count; ++i)
            {
                CustomizableValue customizableValue = list[i];
                if (customizableValue.customId <= 0)
                {
                    customizableValue.customId = maxId + 1;
                    maxId++;
                }
            }
        }

        public static List<CustomizableValue> GetAllCustomizableValue(object obj)
        {
            List<CustomizableValue> list = new List<CustomizableValue>();
            GetAllCustomizableValue(obj, list);
            return list;
        }

        public static void GetAllCustomizableValue(object obj, List<CustomizableValue> list)
        {
            CustomizableValue customizableValue = obj as CustomizableValue;
            if (customizableValue != null)
            {
                list.Add(customizableValue);
            }
            else if (obj is IList)
            {
                foreach (var itemObj in obj as IList)
                {
                    if (itemObj != null)
                    {
                        GetAllCustomizableValue(itemObj, list);
                    }
                }
            }
            else
            {
                Type type = obj.GetType();
                if (type.IsClass || type.IsValueType && !type.IsEnum && !type.IsPrimitive)
                {
                    List<FieldInfo> fieldInfoList = new List<FieldInfo>();
                    fieldInfoList.AddRange(type.GetFields(BindingFlags.Public | BindingFlags.Instance));
                    fieldInfoList.AddRange(type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance));
                    foreach (var fi in fieldInfoList)
                    {
                        object fieldObj = fi.GetValue(obj);
                        if (fieldObj != null)
                        {
                            GetAllCustomizableValue(fieldObj, list);
                        }
                    }
                }
            }
        }
    }
}
