using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MonMoose.Core
{
    [CustomPropertyDrawer(typeof(EnumString), true)]
    public class EnumStringInspector : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            SerializedProperty enumValueProperty = GetEnumValue(property);
            int enumValueIndex = EditorGUI.Popup(position, enumValueProperty.enumValueIndex, enumValueProperty.enumNames);
            ApplyValueIndex(property, enumValueIndex);

            EditorGUI.EndProperty();
        }

        public static SerializedProperty GetEnumValue(SerializedProperty sp)
        {
            return sp.FindPropertyRelative("value");
        }


        public static SerializedProperty GetEnumName(SerializedProperty sp)
        {
            return sp.FindPropertyRelative("m_enumName");
        }

        public static void ApplyValueIndex(SerializedProperty sp, int enumValueIndex)
        {
            SerializedProperty enumValueProperty = GetEnumValue(sp);
            SerializedProperty enumNameProperty = GetEnumName(sp);
            if (enumValueIndex != enumValueProperty.enumValueIndex)
            {
                enumValueProperty.enumValueIndex = enumValueIndex;
                GUI.changed = true;
            }
            string enumStringValue = enumValueProperty.enumNames[enumValueProperty.enumValueIndex];
            if (enumStringValue != enumNameProperty.stringValue)
            {
                enumNameProperty.stringValue = enumStringValue;
                GUI.changed = true;
            }
        }
    }
}