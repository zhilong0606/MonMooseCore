using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MonMoose.Core
{
    public static class SerializationUtility
    {
        public static TypeInfo GetTypeInfoByType(Type type)
        {
            return new TypeInfo
            {
                fullName = type.FullName,
                assemblyName = type.Assembly.GetName().Name,
            };
        }

        static Type GetTypeByTypeInfo(TypeInfo typeInfo)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                if (assembly.GetName().Name == typeInfo.assemblyName)
                {
                    var type = assembly.GetType(typeInfo.fullName);
                    if (type != null)
                    {
                        return type;
                    }
                    break;
                }
            }
            foreach (var assembly in assemblies)
            {
                var type = assembly.GetType(typeInfo.fullName);
                if (type != null)
                {
                    return type;
                }
            }
            return null;
        }

        public static SerializedElement Serialize<T>(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item", "Can not serialize null element");
            }
            var typeInfo = GetTypeInfoByType(item.GetType());
            var data = JsonUtility.ToJson(item, true);
            if (string.IsNullOrEmpty(data))
            {
                throw new ArgumentException(string.Format("Can not serialize {0}", item));
            }
            return new SerializedElement
            {
                typeInfo = typeInfo,
                jsonData = data
            };
        }

        public static T Deserialize<T>(SerializedElement item, params object[] constructorArgs) where T : class
        {
            if (string.IsNullOrEmpty(item.jsonData))
            {
                throw new ArgumentException(string.Format("Can not deserialize {0}, it is invalid", item));
            }
            TypeInfo info = item.typeInfo;
            var type = GetTypeByTypeInfo(info);
            if (type == null)
            {
                throw new ArgumentException(string.Format("Can not deserialize ({0}), type is invalid", info.fullName));
            }
            T instance;
            try
            {
                instance = Activator.CreateInstance(type, constructorArgs) as T;
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Could not construct instance of: {0}", type), e);
            }

            if (instance != null)
            {
                JsonUtility.FromJsonOverwrite(item.jsonData, instance);
                return instance;
            }
            return null;
        }

        public static List<SerializedElement> Serialize<T>(List<T> list)
        {
            var result = new List<SerializedElement>();
            if (list == null)
            {
                return result;
            }
            foreach (var element in list)
            {
                try
                {
                    result.Add(Serialize(element));
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
            return result;
        }

        public static List<T> Deserialize<T>(List<SerializedElement> list) where T : class
        {
            var result = new List<T>();
            if (list == null)
            {
                return result;
            }
            foreach (var element in list)
            {
                try
                {
                    result.Add(Deserialize<T>(element));
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    Debug.LogError(element.jsonData);
                }
            }
            return result;
        }
    }
}