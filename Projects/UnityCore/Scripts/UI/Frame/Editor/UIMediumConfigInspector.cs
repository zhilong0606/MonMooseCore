using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace MonMoose.Core
{
    //[CustomEditor(typeof(UIMediumConfig))]
    public class UIMediumConfigInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            UIMediumConfig config = target as UIMediumConfig;
            if (config == null)
            {
                return;
            }
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("mediumId"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("tagIdList"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("stackOptimizable"));

            bool isDirty = false;
            MonoScript monoScript = AssetWeakRefEditorUtility.PropertyField<MonoScript>("Medium脚本", config.scriptWeakRef, ref isDirty, a => config.scriptWeakRef = a);
            string typeName = monoScript != null ? monoScript.GetClass().Name : string.Empty;
            if (config.typeName != typeName)
            {
                config.typeName = typeName;
                isDirty = true;
            }
            EditorGUILayout.PropertyField(serializedObject.FindProperty("viewConfigList"));
            if (GUILayout.Button("添加并导出"))
            {
            }
            if (EditorGUI.EndChangeCheck() || isDirty)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }
        }
    }
}
