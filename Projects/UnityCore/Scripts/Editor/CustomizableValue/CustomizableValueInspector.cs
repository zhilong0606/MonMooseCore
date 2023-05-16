using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using MonMoose.Core;
using UnityEditor;
using UnityEngine;

namespace MonMoose.Core
{
    [CustomPropertyDrawer(typeof(CustomizableValue), true)]
    public class CustomizableValueInspector : PropertyDrawer
    {
        protected const float m_needCustomToggleWidth = 22f;
        protected const float m_needCustomLabelWidth = 40f;
        protected const string m_needCustomLabelStr = "¿É±ä";

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            GUIContent labelContent = new GUIContent(label);
            labelContent.text = string.Format("{0} ({1})", label.text, property.FindPropertyRelative("customId").intValue);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), labelContent);
            float x = 0f;
            bool needCustom = IsNeedCustom(property);
            EditorGUI.BeginDisabledGroup(needCustom);
            EditorGUI.PropertyField(GetPositionByRate(position, 1f, m_needCustomLabelWidth + m_needCustomToggleWidth, ref x) , property.FindPropertyRelative("specificValue"), new GUIContent(""));
            EditorGUI.EndDisabledGroup();
            EditorGUI.LabelField(GetPositionByFixed(position, m_needCustomLabelWidth, m_needCustomToggleWidth, ref x), m_needCustomLabelStr);
            EditorGUI.PropertyField(GetPositionByFixed(position, m_needCustomLabelWidth, 0f, ref x), property.FindPropertyRelative("needCustom"), new GUIContent(""));
            EditorGUI.EndProperty();
        }

        protected Rect GetPositionByRate(Rect parentRect, float rate, float rightMargin, ref float startPos)
        {
            float width = (parentRect.width - rightMargin) * rate;
            return GetPositionByFixed(parentRect, width, rightMargin, ref startPos);
        }

        protected Rect GetPositionByFixed(Rect parentRect, float fixedWidth, float rightMargin, ref float startPos)
        {
            float width = fixedWidth;
            float x = parentRect.x + startPos;
            startPos = Mathf.Min(startPos + width, parentRect.width + parentRect.x - rightMargin);
            return new Rect(x, parentRect.y, width, parentRect.height);
        }

        private bool IsNeedCustom(SerializedProperty property)
        {
            return property.FindPropertyRelative("needCustom").boolValue;
        }
    }
}
