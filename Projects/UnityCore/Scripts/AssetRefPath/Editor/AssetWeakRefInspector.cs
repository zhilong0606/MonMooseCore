using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MonMoose.Core
{
    [CustomPropertyDrawer(typeof(AssetWeakRef), true)]
    public class AssetWeakRefInspector : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            bool needUpdate;
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            UnityEngine.Object asset = AssetWeakRefEditorUtility.GetAssetByWeakRef<UnityEngine.Object>(property, out needUpdate);
            UnityEngine.Object newAsset = EditorGUI.ObjectField(position, asset, typeof(UnityEngine.Object), false);
            if (newAsset != asset || needUpdate)
            {
                AssetWeakRefEditorUtility.SetAssetWeakRef(property, newAsset);
                GUI.changed = true;
            }
            EditorGUI.EndProperty();
        }
    }
}
