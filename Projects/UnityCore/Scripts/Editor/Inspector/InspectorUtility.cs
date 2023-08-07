using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using MonMoose.GameLogic;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

namespace MonMoose.Core
{
	public static class InspectorUtility
    {
        public static void IntTextField(ref int value, ref bool isDirty, params GUILayoutOption[] options)
        {
            TextFieldTemplate(ref value, ref isDirty, EditorGUILayout.IntField, (a, b) => a == b, options);
        }

        public static void FloatTextField(ref float value, ref bool isDirty, params GUILayoutOption[] options)
        {
            TextFieldTemplate(ref value, ref isDirty, EditorGUILayout.FloatField, (a, b) => Mathf.Abs(a - b) < 1e-4, options);
        }

        public static void StringTextField(ref string value, ref bool isDirty, params GUILayoutOption[] options)
        {
            TextFieldTemplate(ref value, ref isDirty, EditorGUILayout.TextField, (a, b) => a == b, options);
        }

        private static void TextFieldTemplate<T>(ref T value, ref bool isDirty, Func<T, GUILayoutOption[], T> funcOnDrawField, Func<T, T, bool> funcOnCheckSame, params GUILayoutOption[] options)
        {
            T newValue = funcOnDrawField(value, options);
            if (!funcOnCheckSame(newValue, value))
            {
                isDirty = true;
                value = newValue;
            }
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

        public static void DrawInspector(FieldInfo fieldInfo, object obj, ref bool isDirty)
        {
            if (fieldInfo.FieldType == typeof(bool))
            {
                DrawInspectorBool(fieldInfo, obj, ref isDirty);
            }
            if (fieldInfo.FieldType == typeof(int))
            {
                DrawInspectorInt(fieldInfo, obj, ref isDirty);
            }
            if (fieldInfo.FieldType == typeof(float))
            {
                DrawInspectorFloat(fieldInfo, obj, ref isDirty);
            }
            if (fieldInfo.FieldType == typeof(string))
            {
                DrawInspectorString(fieldInfo, obj, ref isDirty);
            }
            if (fieldInfo.FieldType == typeof(AssetWeakRef))
            {
                DrawInspectorAssetWeakRef(fieldInfo, obj, ref isDirty);
            }
            if (typeof(EnumString).IsAssignableFrom(fieldInfo.FieldType))
            {
                DrawInspectorEnumString(fieldInfo, obj, ref isDirty);
            }
            if (typeof(CustomizableValue).IsAssignableFrom(fieldInfo.FieldType))
            {
                DrawInspectorCustomizableValue(fieldInfo, obj, ref isDirty);
            }
            if (typeof(IList).IsAssignableFrom(fieldInfo.FieldType))
            {
                DrawInspectorList(fieldInfo, obj, ref isDirty);
            }
        }

        private static void DrawInspectorBool(FieldInfo fieldInfo, object obj, ref bool isDirty)
        {
            DrawInspectorTemplate<bool>(fieldInfo, obj, EditorGUILayout.Toggle, (x, y) => x == y, ref isDirty);
        }

        private static void DrawInspectorInt(FieldInfo fieldInfo, object obj, ref bool isDirty)
        {
            DrawInspectorTemplate<int>(fieldInfo, obj, EditorGUILayout.IntField, (x, y) => x == y, ref isDirty);
        }

        private static void DrawInspectorFloat(FieldInfo fieldInfo, object obj, ref bool isDirty)
        {
            DrawInspectorTemplate<float>(fieldInfo, obj, EditorGUILayout.FloatField, (x, y) => Math.Abs(x - y) < 1e-4, ref isDirty);
        }

        private static void DrawInspectorString(FieldInfo fieldInfo, object obj, ref bool isDirty)
        {
            DrawInspectorTemplate<string>(fieldInfo, obj, EditorGUILayout.TextField, (x, y) => x == y, ref isDirty);
        }

        private static void DrawInspectorAssetWeakRef(FieldInfo fieldInfo, object obj, ref bool isDirty)
        {
            AssetWeakRefEditorUtility.PropertyField<UnityEngine.Object>(fieldInfo.Name, (AssetWeakRef)fieldInfo.GetValue(obj), ref isDirty, awr => fieldInfo.SetValue(obj, awr));
        }

        private static void DrawInspectorEnum(FieldInfo fieldInfo, object obj, ref bool isDirty)
        {
            if (fieldInfo == null)
            {
                return;
            }
            Type enumType = fieldInfo.FieldType;
            object enumValue = fieldInfo.GetValue(obj);
            List<string> enumNameList;
            List<int> enumValueList;
            GetEnumDefine(enumType, out enumValueList, out enumNameList);
            if(enumNameList.Count == 0)
            {
                return;
            }
            int enumValueIndex = enumValueList.IndexOf((int)enumValue);
            int enumValueIndexNew = EditorGUILayout.Popup(fieldInfo.Name, Mathf.Clamp(enumValueIndex, 0, enumValueList.Count - 1), enumNameList.ToArray());
            if (enumValueIndexNew != enumValueIndex)
            {
                fieldInfo.SetValue(obj, enumValueList[enumValueIndexNew]);
                isDirty = true;
            }
        }

        private static void DrawInspectorEnumString(FieldInfo fieldInfo, object obj, ref bool isDirty)
        {
            if (fieldInfo == null)
            {
                return;
            }
            EnumString enumString = fieldInfo.GetValue(obj) as EnumString;
            if (enumString == null)
            {
                return;
            }
            FieldInfo enumStringValueFieldInfo = enumString.GetType().GetField("value");
            if (enumStringValueFieldInfo == null)
            {
                return;
            }
            object enumStringValue = enumStringValueFieldInfo.GetValue(enumString);
            Type enumType = enumStringValue.GetType();
            var enumArray = Enum.GetValues(enumType);
            if (enumArray.Length == 0)
            {
                return;
            }
            List<string> enumNameList;
            List<int> enumValueList;
            GetEnumDefine(enumType, out enumValueList, out enumNameList);
            if (enumNameList.Count == 0)
            {
                return;
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
            }
        }

        private static void DrawInspectorList(FieldInfo fieldInfo, object obj, ref bool isDirty)
        {
            if (fieldInfo == null)
            {
                return;
            }
            IList list = fieldInfo.GetValue(obj) as IList;
            if (list == null)
            {
                return;
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
        }

        private static void DrawInspectorCustomizableValue(FieldInfo fieldInfo, object obj, ref bool isDirty)
        {
            if (fieldInfo == null)
            {
                return;
            }
            CustomizableValue customizableValue = fieldInfo.GetValue(obj) as CustomizableValue;
            if (customizableValue == null)
            {
                return;
            }
            FieldInfo specificValueFieldInfo = customizableValue.GetType().GetField("specificValue");
            if (specificValueFieldInfo == null)
            {
                return;
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(customizableValue.needCustom);
            DrawInspector(specificValueFieldInfo, customizableValue, ref isDirty);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.LabelField(string.Format("¿É±ä({0})", customizableValue.customId), GUILayout.Width(50));
            bool needCustom = EditorGUILayout.Toggle(customizableValue.needCustom, GUILayout.Width(18));
            if (needCustom != customizableValue.needCustom)
            {
                customizableValue.needCustom = needCustom;
                isDirty = true;
            }
            EditorGUILayout.EndHorizontal();
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

        private static void DrawInspectorTemplate<T>(FieldInfo fieldInfo, object obj, Func<string, T, GUILayoutOption[], T> funcOnDrawField, Func<T, T, bool> funcOnCheckSame, ref bool isDirty)
        {
            if (fieldInfo == null)
            {
                return;
            }
            var value = (T)fieldInfo.GetValue(obj);
            var newValue = funcOnDrawField(fieldInfo.Name, value, new[] { GUILayout.ExpandWidth(true) });
            if (!funcOnCheckSame(newValue, value))
            {
                fieldInfo.SetValue(obj, newValue);
                isDirty = true;
            }
        }
    }
}
