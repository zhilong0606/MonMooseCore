using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MonMoose.Core
{
    public static class UIBindUtility
    {
        private static List<UIBindItemTypeInfo> m_bindItemTypeInfoList = new List<UIBindItemTypeInfo>();

        static UIBindUtility()
        {
            m_bindItemTypeInfoList.Add(new UIBindItemTypeInfo(UIBindItemType.SubBindGroup, typeof(UIBindGroup)));

            m_bindItemTypeInfoList.Add(new UIBindItemTypeInfo(UIBindItemType.GameObjectPool, typeof(GameObjectPool)));
            m_bindItemTypeInfoList.Add(new UIBindItemTypeInfo(UIBindItemType.AnimationController, typeof(AnimationController)));

            m_bindItemTypeInfoList.Add(new UIBindItemTypeInfo(UIBindItemType.Button, typeof(Button)));
            m_bindItemTypeInfoList.Add(new UIBindItemTypeInfo(UIBindItemType.Toggle, typeof(Toggle)));
            m_bindItemTypeInfoList.Add(new UIBindItemTypeInfo(UIBindItemType.Image, typeof(Image)));
            m_bindItemTypeInfoList.Add(new UIBindItemTypeInfo(UIBindItemType.Text, typeof(Text)));
            m_bindItemTypeInfoList.Add(new UIBindItemTypeInfo(UIBindItemType.TextMeshPro, typeof(TextMeshProUGUI)));
        }

        public static UIBindItemType GetMatchedBindType(GameObject go)
        {
            for (int i = 0; i < m_bindItemTypeInfoList.Count; ++i)
            {
                if (go.GetComponent(m_bindItemTypeInfoList[i].classType) != null)
                {
                    return m_bindItemTypeInfoList[i].bindType;
                }
            }
            return default;
        }

        public static UnityEngine.Object GetBindComponent(UIBindItemInfo itemInfo)
        {
            GameObject bindObj = itemInfo.bindObj;
            if (bindObj == null)
            {
                return null;
            }
            if(itemInfo.bindType.value == UIBindItemType.GameObject)
            {
                return bindObj;
            }
            UIBindItemTypeInfo typeInfo = GetBindItemTypeInfo(itemInfo.bindType.value);
            if (typeInfo != null)
            {
                return bindObj.GetComponent(typeInfo.classType);
            }
            return null;
        }

        public static UnityEngine.Object GetBindComponent(UIBindItemInfo itemInfo, Type type)
        {
            if (itemInfo == null)
            {
                return null;
            }
            GameObject bindObj = itemInfo.bindObj;
            if (bindObj == null || type == null)
            {
                return null;
            }
            return bindObj.GetComponent(type);
        }

        public static GameObject GetBindObject(UIBindItemInfo itemInfo)
        {
            if(itemInfo == null)
            {
                return null;
            }
            return itemInfo.bindObj;
        }

        public static string GetBindTypeName(UIBindItemType bindType)
        {
            if (bindType == UIBindItemType.GameObject)
            {
                return typeof(GameObject).Name;
            }
            UIBindItemTypeInfo typeInfo = GetBindItemTypeInfo(bindType);
            if (typeInfo != null)
            {
                return typeInfo.classType.Name;
            }
            return string.Empty;
        }

        public static string GetBindTypeNamespace(UIBindItemType bindType)
        {
            UIBindItemTypeInfo typeInfo = GetBindItemTypeInfo(bindType);
            if(bindType == UIBindItemType.GameObject)
            {
                return typeof(GameObject).Namespace;
            }
            if (typeInfo != null)
            {
                return typeInfo.classType.Namespace;
            }
            return string.Empty;
        }

        public static UIBindItemTypeInfo GetBindItemTypeInfo(UIBindItemType bindType)
        {
            for (int i = 0; i < m_bindItemTypeInfoList.Count; ++i)
            {
                if(m_bindItemTypeInfoList[i].bindType == bindType)
                {
                    return m_bindItemTypeInfoList[i];
                }
            }
            return null;
        }
    }
}
