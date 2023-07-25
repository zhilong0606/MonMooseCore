using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

namespace MonMoose.Core
{
	public static class InspectorUtility
	{
        public static bool FloatTextField(float value, out float newValue, params GUILayoutOption[] options)
        {
            newValue = EditorGUILayout.FloatField(value, options);
            if (Mathf.Abs(newValue - value) >= 1e-4)
            {
                return true;
            }
            newValue = value;
            return false;
        }

        public static void DrawInspector(object obj, ref bool isDirty)
        {
            List<FieldInfo> fieldInfoList = new List<FieldInfo>();
            List<FieldInfo> forIndexList = new List<FieldInfo>();
            foreach (var fi in obj.GetType().GetFields())
            {
                if (!fi.IsPublic || fi.IsStatic)
                {
                    continue;
                }
                fieldInfoList.Add(fi);
                forIndexList.Add(fi);
            }
            fieldInfoList.Sort((x, y) =>
            {
                if (x.DeclaringType != null && y.DeclaringType != null && x.DeclaringType != y.DeclaringType)
                {
                    return x.DeclaringType.IsAssignableFrom(y.DeclaringType) ? -1 : 1;
                }
                return forIndexList.IndexOf(x).CompareTo(forIndexList.IndexOf(y));
            });
            foreach (var fi in fieldInfoList)
            {
                DrawInspector(fi, obj, ref isDirty);
            }
        }

        public static bool DrawInspector(FieldInfo fieldInfo, object obj, ref bool isDirty)
        {
            if (fieldInfo.FieldType == typeof(bool))
            {
                return DrawInspectorBool(fieldInfo, obj, ref isDirty);
            }
            if (fieldInfo.FieldType == typeof(int))
            {
                return DrawInspectorInt(fieldInfo, obj, ref isDirty);
            }
            if (fieldInfo.FieldType == typeof(float))
            {
                return DrawInspectorFloat(fieldInfo, obj, ref isDirty);
            }
            if (fieldInfo.FieldType == typeof(string))
            {
                return DrawInspectorString(fieldInfo, obj, ref isDirty);
            }
            if (typeof(EnumString).IsAssignableFrom(fieldInfo.FieldType))
            {
                return DrawInspectorEnumString(fieldInfo, obj, ref isDirty);
            }
            if (typeof(CustomizableValue).IsAssignableFrom(fieldInfo.FieldType))
            {
                return DrawInspectorCustomizableValue(fieldInfo, obj, ref isDirty);
            }
            if (typeof(IList).IsAssignableFrom(fieldInfo.FieldType))
            {
                return DrawInspectorList(fieldInfo, obj, ref isDirty);
            }
            return false;
        }

        private static bool DrawInspectorBool(FieldInfo fieldInfo, object obj, ref bool isDirty)
        {
            return DrawInspectorTemplate<bool>(fieldInfo, obj, EditorGUILayout.Toggle, (x, y) => x == y, ref isDirty);
        }

        private static bool DrawInspectorInt(FieldInfo fieldInfo, object obj, ref bool isDirty)
        {
            return DrawInspectorTemplate<int>(fieldInfo, obj, EditorGUILayout.IntField, (x, y) => x == y, ref isDirty);
        }

        private static bool DrawInspectorFloat(FieldInfo fieldInfo, object obj, ref bool isDirty)
        {
            return DrawInspectorTemplate<float>(fieldInfo, obj, EditorGUILayout.FloatField, (x, y) => Math.Abs(x - y) < 1e-4, ref isDirty);
        }

        private static bool DrawInspectorString(FieldInfo fieldInfo, object obj, ref bool isDirty)
        {
            return DrawInspectorTemplate<string>(fieldInfo, obj, EditorGUILayout.TextField, (x, y) => x == y, ref isDirty);
        }

        private static bool DrawInspectorEnum(FieldInfo fieldInfo, object obj, ref bool isDirty)
        {
            if (fieldInfo == null)
            {
                return false;
            }
            Type enumType = fieldInfo.FieldType;
            object enumValue = fieldInfo.GetValue(obj);
            List<string> enumNameList;
            List<int> enumValueList;
            GetEnumDefine(enumType, out enumValueList, out enumNameList);
            if(enumNameList.Count == 0)
            {
                return false;
            }
            int enumValueIndex = enumValueList.IndexOf((int)enumValue);
            int enumValueIndexNew = EditorGUILayout.Popup(fieldInfo.Name, Mathf.Clamp(enumValueIndex, 0, enumValueList.Count - 1), enumNameList.ToArray());
            if (enumValueIndexNew != enumValueIndex)
            {
                fieldInfo.SetValue(obj, enumValueList[enumValueIndexNew]);
                isDirty = true;
            }
            return true;
        }

        private static bool DrawInspectorEnumString(FieldInfo fieldInfo, object obj, ref bool isDirty)
        {
            if (fieldInfo == null)
            {
                return false;
            }
            EnumString enumString = fieldInfo.GetValue(obj) as EnumString;
            if (enumString == null)
            {
                return false;
            }
            FieldInfo enumStringValueFieldInfo = enumString.GetType().GetField("value");
            if (enumStringValueFieldInfo == null)
            {
                return false;
            }
            object enumStringValue = enumStringValueFieldInfo.GetValue(enumString);
            Type enumType = enumStringValue.GetType();
            var enumArray = Enum.GetValues(enumType);
            if (enumArray.Length == 0)
            {
                return false;
            }
            List<string> enumNameList;
            List<int> enumValueList;
            GetEnumDefine(enumType, out enumValueList, out enumNameList);
            if (enumNameList.Count == 0)
            {
                return false;
            }
            FieldInfo enumStringNameFieldInfo = enumString.GetType().GetField("m_enumName", BindingFlags.NonPublic | BindingFlags.Instance);
            if (enumStringNameFieldInfo != null)
            {
                int enumStringValueIndex = enumValueList.IndexOf((int)enumStringValue);
                int enumStringValueIndexNew = EditorGUILayout.Popup(fieldInfo.Name, Mathf.Clamp(enumStringValueIndex, 0, enumArray.Length - 1), enumNameList.ToArray());
                if (enumStringValueIndexNew != enumStringValueIndex)
                {
                    enumStringValueFieldInfo.SetValue(enumString, enumValueList[enumStringValueIndexNew]);
                    enumStringNameFieldInfo.SetValue(enumString, enumValueList[enumStringValueIndexNew].ToString());
                    isDirty = true;
                }
                return true;
            }
            return false;
        }

        private static bool DrawInspectorList(FieldInfo fieldInfo, object obj, ref bool isDirty)
        {
            if (fieldInfo == null)
            {
                return false;
            }
            IList list = fieldInfo.GetValue(obj) as IList;
            if (list == null)
            {
                return false;
            }
            SirenixEditorGUI.BeginHorizontalToolbar();
            //this.visible.Value = SirenixEditorGUI.Foldout(this.visible.Value, GUIHelper.TempContent("SyncList " + label.text + "  [" + typeof(TList).Name + "]"));
            EditorGUILayout.LabelField(GUIHelper.TempContent("Empty items () items"), SirenixGUIStyles.RightAlignedGreyMiniLabel);
            SirenixEditorGUI.EndHorizontalToolbar();

            if (SirenixEditorGUI.BeginFadeGroup(list, true))
            {
                SirenixEditorGUI.BeginVerticalList();
                {
                    var elementLabel = new GUIContent();
                    for (int i = 0; i < list.Count; i++)
                    {
                        SirenixEditorGUI.BeginListItem();
                        elementLabel.text = "Item " + i;

                        DrawInspector(list[i], ref isDirty);
                        //if (i < minCount)
                        //{
                        //    property.Children[i].Draw(elementLabel);
                        //}
                        //else
                        //{
                        //    EditorGUILayout.LabelField(elementLabel, SirenixEditorGUI.MixedValueDashChar);
                        //}
                        SirenixEditorGUI.EndListItem();
                    }
                }
                SirenixEditorGUI.EndVerticalList();
            }
            SirenixEditorGUI.EndFadeGroup();
            return true;
        }

        private static bool DrawInspectorCustomizableValue(FieldInfo fieldInfo, object obj, ref bool isDirty)
        {
            if (fieldInfo == null)
            {
                return false;
            }
            CustomizableValue customizableValue = fieldInfo.GetValue(obj) as CustomizableValue;
            if (customizableValue == null)
            {
                return false;
            }
            FieldInfo specificValueFieldInfo = customizableValue.GetType().GetField("specificValue");
            if (specificValueFieldInfo == null)
            {
                return false;
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(customizableValue.needCustom);
            DrawInspector(specificValueFieldInfo, customizableValue, ref isDirty);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.LabelField(string.Format("�ɱ�({0})", customizableValue.customId), GUILayout.Width(50));
            bool needCustom = EditorGUILayout.Toggle(customizableValue.needCustom, GUILayout.Width(18));
            if (needCustom != customizableValue.needCustom)
            {
                customizableValue.needCustom = needCustom;
                isDirty = true;
            }
            EditorGUILayout.EndHorizontal();
            return true;
        }

        private static void GetEnumDefine(Type type, out List<int> valueList, out List<string> nameList)
        {
            valueList = new List<int>();
            nameList = new List<string>();
            var enumArray = Enum.GetValues(type);
            if (enumArray.Length == 0)
            {
                return;
            }
            foreach (var enumValue in enumArray)
            {
                valueList.Add((int)enumValue);
                nameList.Add(enumValue.ToString());
            }
        }

        private static bool DrawInspectorTemplate<T>(FieldInfo fieldInfo, object obj, Func<string, T, GUILayoutOption[], T> funcOnDrawField, Func<T, T, bool> funcOnCheckSame, ref bool isDirty)
        {
            if (fieldInfo == null)
            {
                return false;
            }
            var value = (T)fieldInfo.GetValue(obj);
            var newValue = funcOnDrawField(fieldInfo.Name, value, new[] { GUILayout.ExpandWidth(true) });
            if (!funcOnCheckSame(newValue, value))
            {
                fieldInfo.SetValue(obj, newValue);
                isDirty = true;
            }
            return true;
        }
    }
}
