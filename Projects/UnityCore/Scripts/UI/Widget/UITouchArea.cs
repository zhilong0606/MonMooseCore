using UnityEngine;
using UnityEngine.UI;

namespace MonMoose.Core
{
    public class UITouchArea : Image
    {
        public bool showColor;
        private Collider2D[] touchCollider = new Collider2D[0];

        protected override void Awake()
        {
            touchCollider = GetComponents<Collider2D>();
            base.Awake();
        }

        public override bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            if (touchCollider.Length > 0)
            {
                for (int i = 0; i < touchCollider.Length; ++i)
                {
                    if (touchCollider[i].OverlapPoint(eventCamera.ScreenToWorldPoint(sp)))
                    {
                        return true;
                    }
                }
            }
            else if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, sp, eventCamera))
            {
                return true;
            }
            return false;
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            if (showColor)
            {
                base.OnPopulateMesh(vh);
            }
            else
            {
                vh.Clear();
            }
        }
    }
}
