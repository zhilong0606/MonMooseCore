using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace MonMoose.Core
{
    public class UIExceptTouchArea : Image
    {
        public List<RectTransform> exceptList = new List<RectTransform>();
        public Action OnTouched;

        public override bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, sp, eventCamera))
            {
                if (Input.touchCount > 0
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
                    || Input.GetMouseButton(0)
#endif
                )
                {
                    bool valid = true;
                    for (int i = 0; i < exceptList.Count; ++i)
                    {
                        if (RectTransformUtility.RectangleContainsScreenPoint(exceptList[i], sp, eventCamera))
                        {
                            valid = false;
                            break;
                        }
                    }
                    if (valid && OnTouched != null)
                    {
                        OnTouched();
                        gameObject.SetActiveSafely(false);
                    }
                }
            }
            return false;
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
        }
    }
}
