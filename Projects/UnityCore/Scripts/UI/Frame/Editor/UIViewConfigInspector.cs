using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MonMoose.Core
{
    //[CustomPropertyDrawer(typeof(UIViewConfig), true)]
    public class UIViewConfigInspector : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            float startPos = 0f;
            EditorGUI.PropertyField(GetPositionByRate(position, 0.5f, Vector2.zero, new Vector2(0, 5), ref startPos), property.FindPropertyRelative("nameStr"), GUIContent.none);

            bool needUpdateMonoScript;
            GameObject prefab = AssetWeakRefEditorUtility.GetAssetByWeakRef<GameObject>(property.FindPropertyRelative("prefabWeakRef"), out needUpdateMonoScript);
            GameObject newPrefab = EditorGUI.ObjectField(GetPositionByRate(position, 0.5f, Vector2.zero, new Vector2(0, 5), ref startPos), GUIContent.none, prefab, typeof(GameObject), false) as GameObject;
            bool isDirty = false;
            if (newPrefab != prefab || needUpdateMonoScript)
            {
                prefab = newPrefab;
                AssetWeakRefEditorUtility.SetAssetWeakRef(property.FindPropertyRelative("prefabWeakRef"), prefab);
                isDirty = true;
            }
            if (prefab != null)
            {
                UIBindGroup bindGroup = prefab.GetComponent<UIBindGroup>();
                if (bindGroup != null)
                {
                    MonoScript monoScript = AssetWeakRefEditorUtility.GetAssetByWeakRef<MonoScript>(bindGroup.scriptWeakRef);
                    string typeName = monoScript != null ? monoScript.GetClass().Name : string.Empty;
                    if (property.FindPropertyRelative("typeName").stringValue != typeName)
                    {
                        property.FindPropertyRelative("typeName").stringValue = typeName;
                        isDirty = true;
                    }
                }
            }
            if (isDirty)
            {
                EditorUtility.SetDirty(property.serializedObject.targetObject);
                AssetDatabase.SaveAssets();
            }
            EditorGUI.EndProperty();
        }


        protected Rect GetPositionByRate(Rect parentRect, float rate, Vector2 parentMargin, Vector2 selfMargin, ref float startPos)
        {
            parentMargin.x = Mathf.Max(0, parentMargin.x);
            parentMargin.y = Mathf.Max(0, parentMargin.y);
            selfMargin.x = Mathf.Max(0, selfMargin.x);
            selfMargin.y = Mathf.Max(0, selfMargin.y);
            float parentUsableWidth = parentRect.width - parentMargin.x - parentMargin.y;
            float selfWidth = parentUsableWidth * rate;
            Rect resultPosition = GetPosition(parentRect, startPos + selfMargin.x, selfWidth - selfMargin.x - selfMargin.y);
            startPos += selfWidth;
            return resultPosition;
        }

        protected Rect GetPosition(Rect parentRect, float startPos, float width)
        {
            float x = parentRect.x + startPos;
            return new Rect(x, parentRect.y, width, parentRect.height);
        }
    }
}
