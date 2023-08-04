using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MonMoose.Core.ClassCollect
{
    public static class ClassCollectUtility
    {
        private static readonly List<Type> m_exceptCollectTypeList = new List<Type>()
        {
            typeof(string),
        };

        public static void CollectClass(Type type, List<ClassInfo> classList, List<Type> exceptTypeList)
        {
            List<FieldInfo> exceptFieldInfoList = new List<FieldInfo>();
            if (exceptTypeList != null)
            {
                foreach (var exceptType in exceptTypeList)
                {
                    CollectFieldInfo(exceptType, exceptFieldInfoList);
                }
            }
            CollectClass(type, classList, exceptFieldInfoList);
        }

        public static void CollectClass(Type type, List<ClassInfo> classList, List<FieldInfo> exceptFieldInfoList)
        {
            if (type.IsGenericType)
            {
                foreach (var gt in type.GetGenericArguments())
                {
                    CollectClassInternal(gt, classList, exceptFieldInfoList);
                }
            }
            else
            {
                CollectClassInternal(type, classList, exceptFieldInfoList);
            }
        }

        private static void CollectClassInternal(Type type, List<ClassInfo> classList, List<FieldInfo> exceptFieldInfoList)
        {
            if (!CheckTypeNeedCollect(type))
            {
                return;
            }
            if (classList.Find(c => c.type == type) != null)
            {
                return;
            }
            ClassInfo classInfo = new ClassInfo();
            classInfo.type = type;
            classList.Add(classInfo);
            foreach (var fieldInfo in type.GetFields())
            {
                if (exceptFieldInfoList != null && exceptFieldInfoList.Contains(fieldInfo))
                {
                    continue;
                }
                string memberName = fieldInfo.Name;
                ClassMemberInfo memberInfo = new ClassMemberInfo();
                memberInfo.name = memberName;
                memberInfo.type = fieldInfo.FieldType;
                memberInfo.fieldInfo = fieldInfo;
                classInfo.memberList.Add(memberInfo);
                CollectClass(fieldInfo.FieldType, classList, exceptFieldInfoList);
            }
        }

        private static void CollectFieldInfo(Type type, List<FieldInfo> list)
        {
            foreach (var fieldInfo in type.GetFields())
            {
                list.Add(fieldInfo);
            }
        }

        private static bool CheckTypeNeedCollect(Type type)
        {
            if (!type.IsClass)
            {
                return false;
            }
            if (m_exceptCollectTypeList.Contains(type))
            {
                return false;
            }
            return true;
        }
    }
}
