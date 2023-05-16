using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Sprites;

namespace MonMoose.Core
{
    [AddComponentMenu("UI/UIMaskImage", 11)]
    public class UIMaskImage : Image
    {
        /// Image's dimensions used for drawing. X = left, Y = bottom, Z = right, W = top.
        private Vector4 GetDrawingDimensions(bool shouldPreserveAspect)
        {
            var padding = overrideSprite == null ? Vector4.zero : DataUtility.GetPadding(overrideSprite);
            var size = overrideSprite == null ? Vector2.zero : new Vector2(overrideSprite.rect.width, overrideSprite.rect.height);

            Rect r = GetPixelAdjustedRect();
            // Debug.Log(string.Format("r:{2}, size:{0}, padding:{1}", size, padding, r));

            int spriteW = Mathf.RoundToInt(size.x);
            int spriteH = Mathf.RoundToInt(size.y);

            var v = new Vector4(
                    padding.x / spriteW,
                    padding.y / spriteH,
                    (spriteW - padding.z) / spriteW,
                    (spriteH - padding.w) / spriteH);

            if (shouldPreserveAspect && size.sqrMagnitude > 0.0f)
            {
                var spriteRatio = size.x / size.y;
                var rectRatio = r.width / r.height;

                if (spriteRatio > rectRatio)
                {
                    var oldHeight = r.height;
                    r.height = r.width * (1.0f / spriteRatio);
                    r.y += (oldHeight - r.height) * rectTransform.pivot.y;
                }
                else
                {
                    var oldWidth = r.width;
                    r.width = r.height * spriteRatio;
                    r.x += (oldWidth - r.width) * rectTransform.pivot.x;
                }
            }

            v = new Vector4(
                    r.x + r.width * v.x,
                    r.y + r.height * v.y,
                    r.x + r.width * v.z,
                    r.y + r.height * v.w
                    );

            return v;
        }

        /// <summary>
        /// Update the UI renderer mesh.
        /// </summary>
        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            if (overrideSprite == null)
            {
                base.OnPopulateMesh(toFill);
                return;
            }

            switch (type)
            {
                case Type.Simple:
                    GenerateSimpleSprite(toFill, preserveAspect);
                    break;
                default:
                    base.OnPopulateMesh(toFill);
                    break;
            }
        }

        /// <summary>
        /// Generate vertices for a simple Image.
        /// </summary>
        private void GenerateSimpleSprite(VertexHelper vh, bool lPreserveAspect)
        {
            Vector4 v = GetDrawingDimensions(lPreserveAspect);
            var uv = (overrideSprite != null) ? DataUtility.GetOuterUV(overrideSprite) : Vector4.zero;

            var color32 = color;
            vh.Clear();
            vh.AddVert(new Vector3(v.x, v.y), color32, new Vector2(uv.x, uv.y), new Vector2(0, 0), Vector3.zero, Vector4.zero);
            vh.AddVert(new Vector3(v.x, v.w), color32, new Vector2(uv.x, uv.w), new Vector2(0, 1), Vector3.zero, Vector4.zero);
            vh.AddVert(new Vector3(v.z, v.w), color32, new Vector2(uv.z, uv.w), new Vector2(1, 1), Vector3.zero, Vector4.zero);
            vh.AddVert(new Vector3(v.z, v.y), color32, new Vector2(uv.z, uv.y), new Vector2(1, 0), Vector3.zero, Vector4.zero);

            vh.AddTriangle(0, 1, 2);
            vh.AddTriangle(2, 3, 0);
        }
    }
}
