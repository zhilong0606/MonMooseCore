#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MonMoose.Core
{
    [CustomEditor(typeof(UIGradient))]
    public class UIGradientInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_color1"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_color2"));
            DrawLabeledFloatSlider(serializedObject, "Dir", "m_dir", -90f, 90f);
            DrawMinMaxLabeledFloatSlider(serializedObject, "StartEnd", "m_startPos", "m_endPos", 0, 1f);
            if (GUILayout.Button("Exchange"))
            {
                Color temp = serializedObject.FindProperty("m_color1").colorValue;
                serializedObject.FindProperty("m_color1").colorValue = serializedObject.FindProperty("m_color2").colorValue;
                serializedObject.FindProperty("m_color2").colorValue = temp;
            }
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                if (EditorApplication.isPlaying)
                {
                    return;
                }
                EditorUtility.SetDirty(target);
            }
        }

        public static void DrawLabeledFloatSlider(SerializedObject serializedObject, string name, string propertyName, float minLimit, float maxLimit)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Slider(property, minLimit, maxLimit, name);
            EditorGUILayout.EndHorizontal();
        }

        public static void DrawMinMaxLabeledFloatSlider(SerializedObject serializedObject, string name, string minName, string maxName, float minLimit, float maxLimit)
        {
            SerializedProperty minProperty = serializedObject.FindProperty(minName);
            SerializedProperty maxProperty = serializedObject.FindProperty(maxName);
            float minValue = Mathf.Round(minProperty.floatValue * 100f) / 100f;
            float maxValue = Mathf.Round(maxProperty.floatValue * 100f) / 100f;
            EditorGUILayout.BeginHorizontal();
            minValue = EditorGUILayout.FloatField(name, minValue);
            EditorGUILayout.MinMaxSlider(ref minValue, ref maxValue, minLimit, maxLimit);
            maxValue = EditorGUILayout.FloatField(maxValue);
            EditorGUILayout.EndHorizontal();
            minProperty.floatValue = minValue;
            maxProperty.floatValue = maxValue;
        }
    }
}
#endif