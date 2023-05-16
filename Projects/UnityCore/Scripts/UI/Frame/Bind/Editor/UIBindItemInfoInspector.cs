using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MonMoose.Core
{
    [CustomPropertyDrawer(typeof(UIBindItemInfo), true)]
    public class UIBindItemInfoInspector : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            float startPos = 0f;
            //EditorGUI.PropertyField(GetPositionByRate(position, 0.4f, Vector2.zero, new Vector2(0, 5), ref startPos), property.FindPropertyRelative("remarkStr"), GUIContent.none);
            SerializedProperty nameStrSp = property.FindPropertyRelative("nameStr");
            EditorGUI.PropertyField(GetPositionByRate(position, 0.3f, Vector2.zero, new Vector2(0, 5), ref startPos), nameStrSp, GUIContent.none);
            SerializedProperty bindObjSp = property.FindPropertyRelative("bindObj");
            GameObject preBindObj = bindObjSp.objectReferenceValue as GameObject;
            EditorGUI.PropertyField(GetPositionByRate(position, 0.3f, Vector2.zero, new Vector2(0, 5), ref startPos), bindObjSp, GUIContent.none);
            SerializedProperty bindTypeSp = property.FindPropertyRelative("bindType");
            EditorGUI.PropertyField(GetPositionByRate(position, 0.4f, Vector2.zero, new Vector2(0, 5), ref startPos), bindTypeSp, GUIContent.none);

            GameObject curBindObj = bindObjSp.objectReferenceValue as GameObject;
            if (curBindObj != preBindObj)
            {
                UIBindItemType curBindType = (UIBindItemType)EnumStringInspector.GetEnumValue(bindTypeSp).enumValueIndex;
                if (curBindType == UIBindItemType.None)
                {
                    UIBindItemType matchedBindType = UIBindUtility.GetMatchedBindType(curBindObj);
                    EnumStringInspector.ApplyValueIndex(bindTypeSp, (int)matchedBindType);
                }
                string curNameStr = nameStrSp.stringValue;
                if (string.IsNullOrEmpty(curNameStr))
                {
                    nameStrSp.stringValue = curBindObj.name;
                    GUI.changed = true;
                }
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
