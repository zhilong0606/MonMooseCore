#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MonMoose.Core
{
    public static class UISizeTool
    {

        [MenuItem("Tools/UI/Set Fit Size")]
        private static void SetFitSize()
        {
            for (int i = 0; i < Selection.gameObjects.Length; ++i)
            {
                GameObject go = Selection.gameObjects[i];
                RectTransform trans = go.GetComponent<RectTransform>();
                if (trans == null)
                {
                    continue;
                }
                RectTransform parent = trans.parent as RectTransform;
                if (parent == null)
                {
                    continue;
                }
                float parentWidth = parent.rect.width;
                float parentHeight = parent.rect.height;
                float width = trans.rect.width;
                float height = trans.rect.height;
                Debug.LogError(trans.localPosition);
                Debug.LogError(trans.anchoredPosition);
                float xMin = trans.localPosition.x - trans.pivot.x * width;
                float xMax = trans.localPosition.x + (1 - trans.pivot.x) * width;
                float yMin = trans.localPosition.y - trans.pivot.y * height;
                float yMax = trans.localPosition.y + (1 - trans.pivot.y) * height;
                trans.anchorMin = new Vector2(1 + xMin / parentWidth - 0.5f, 1 + yMin / parentHeight - 0.5f);
                trans.anchorMax = new Vector2(xMax / parentWidth + 0.5f, yMax / parentHeight + 0.5f);
                trans.offsetMin = Vector2.zero;
                trans.offsetMax = Vector2.zero;

            }
        }

    }
}
#endif