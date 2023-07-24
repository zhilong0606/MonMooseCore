using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace MonMoose.Core
{
    [CustomEditor(typeof(UIBindGroup))]
    public class UIBindGroupInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            UIBindGroup group = target as UIBindGroup;
            if (group == null)
            {
                return;
            }
            EditorGUI.BeginChangeCheck();

            bool isDirty = false;
            AssetWeakRefEditorUtility.PropertyField<MonoScript>("ControllerΩ≈±æ", group.scriptWeakRef, ref isDirty, a => group.scriptWeakRef = a);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("bindItemList"));
            if (GUILayout.Button("Generate Bind Code"))
            {
                List<string> errorList = new List<string>();
                if (!UIBindEditorUtility.CheckBindGroupValid(group, errorList))
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (string str in errorList)
                    {
                        sb.AppendLine(str);
                    }
                    EditorUtility.DisplayDialog("¥ÌŒÛ", sb.ToString(), "∂Æ¡À");
                }
                else
                {
                    UIBindEditorUtility.GenerateBindCode(group);
                }
            }
            if (EditorGUI.EndChangeCheck() || isDirty)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }
        }
    }
}
