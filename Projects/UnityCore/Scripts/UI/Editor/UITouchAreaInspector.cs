#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MonMoose.Core
{
    [CustomEditor(typeof(UITouchArea))]
    [CanEditMultipleObjects]
    public class UITouchAreaInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("showColor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Color"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_RaycastTarget"));
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }
        }
    }
}
#endif