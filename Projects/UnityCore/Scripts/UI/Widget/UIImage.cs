using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Sprites;


namespace MonMoose.Core
{
    public enum ETextureLayout
    {
        None,
        Horizonatal,
        Vertical
    }

    [AddComponentMenu("UI/UIImage", 11)]
    public class UIImage : Image
    {
        [SerializeField] private ETextureLayout m_textureLayout;

        protected static Vector2[] s_sizeScaling = new Vector2[]
        {
            new Vector2(1f, 1f),
            new Vector2(0.5f, 1f),
            new Vector2(1f, 0.5f)
        };

        public ETextureLayout TextureLayout
        {
            get { return m_textureLayout; }
            set
            {
                if (m_textureLayout != value)
                {
                    m_textureLayout = value;
                    SetMaterialDirty();
                }
            }
        }

        /// Image's dimensions used for drawing. X = left, Y = bottom, Z = right, W = top.
        protected Vector4 GetDrawingDimensions(bool shouldPreserveAspect, Vector2 sizeScaling)
        {
            var padding = overrideSprite == null ? Vector4.zero : DataUtility.GetPadding(overrideSprite);
            var size = overrideSprite == null ? Vector2.zero : new Vector2(overrideSprite.rect.width * sizeScaling.x, overrideSprite.rect.height * sizeScaling.y);

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

        public override void SetNativeSize()
        {
            if (overrideSprite != null)
            {
                Vector2 vector = s_sizeScaling[(int)TextureLayout];
                float w = overrideSprite.rect.width * vector.x / pixelsPerUnit;
                float h = overrideSprite.rect.height * vector.y / pixelsPerUnit;
                rectTransform.anchorMax = rectTransform.anchorMin;
                rectTransform.sizeDelta = new Vector2(w, h);
                SetAllDirty();
            }
        }

        /// <summary>
        /// Update the UI renderer mesh.
        /// </summary>
        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            if (overrideSprite == null)
            {
                toFill.Clear();
                //base.OnPopulateMesh(toFill);
                return;
            }

            switch (type)
            {
                case Type.Simple:
                    GenerateSimpleSprite(toFill, preserveAspect);
                    break;
                case Type.Sliced:
                    GenerateSlicedSprite(toFill);
                    break;
                case Type.Filled:
                    GenerateFilledSprite(toFill, preserveAspect);
                    break;
                case Type.Tiled:
                    GenerateTiledSprite(toFill);
                    break;
                    ;
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
            Vector4 v = GetDrawingDimensions(lPreserveAspect, s_sizeScaling[(int)TextureLayout]);
            Vector4 uv = (overrideSprite != null) ? DataUtility.GetOuterUV(overrideSprite) : Vector4.zero;

            float lb_x = uv.x;
            float lb_y = uv.y;
            float rt_x = uv.z;
            float rt_y = uv.w;

            float c1_x = (TextureLayout == ETextureLayout.Horizonatal) ? ((lb_x + rt_x) * 0.5f) : lb_x;
            float c1_y = (TextureLayout == ETextureLayout.Horizonatal) ? lb_y : ((lb_y + rt_y) * 0.5f);
            float c2_x = (TextureLayout == ETextureLayout.Horizonatal) ? ((lb_x + rt_x) * 0.5f) : rt_x;
            float c2_y = (TextureLayout == ETextureLayout.Horizonatal) ? rt_y : ((lb_y + rt_y) * 0.5f);

            Color color32 = color;
            vh.Clear();

            vh.AddVert(new Vector3(v.x, v.y), color32, new Vector2(lb_x, lb_y), new Vector2(c1_x, c1_y), Vector3.zero,
                Vector4.zero);
            vh.AddVert(new Vector3(v.x, v.w), color32, new Vector2(lb_x, c2_y), new Vector2(c1_x, rt_y), Vector3.zero,
                Vector4.zero);
            vh.AddVert(new Vector3(v.z, v.w), color32, new Vector2(c2_x, c2_y), new Vector2(rt_x, rt_y), Vector3.zero,
                Vector4.zero);
            vh.AddVert(new Vector3(v.z, v.y), color32, new Vector2(c2_x, lb_y), new Vector2(rt_x, c1_y), Vector3.zero,
                Vector4.zero);

            vh.AddTriangle(0, 1, 2);
            vh.AddTriangle(2, 3, 0);
        }

        /// <summary>
        /// Generate vertices for a 9-sliced Image.
        /// </summary>

        static readonly Vector2[] s_VertScratch = new Vector2[4];

        static readonly Vector2[] s_UVScratch = new Vector2[4];

        private void GenerateSlicedSprite(VertexHelper toFill)
        {
            if (!hasBorder)
            {
                GenerateSimpleSprite(toFill, false);
                return;
            }

            Vector2 offset = Vector2.zero;
            Vector4 outer, inner, padding, border;

            if (overrideSprite != null)
            {
                //outer = DataUtility.GetOuterUV(overrideSprite);
                //inner = DataUtility.GetInnerUV(overrideSprite);
                outer = GetOuterUV(overrideSprite, m_textureLayout, out offset);
                inner = GetInnerUV(overrideSprite, s_sizeScaling[(int)TextureLayout]);
                padding = DataUtility.GetPadding(overrideSprite);
                border = overrideSprite.border;
            }
            else
            {
                outer = Vector4.zero;
                inner = Vector4.zero;
                padding = Vector4.zero;
                border = Vector4.zero;
            }

            Rect rect = GetPixelAdjustedRect();
            border = GetAdjustedBorders(border / pixelsPerUnit, rect);
            padding = padding / pixelsPerUnit;

            s_VertScratch[0] = new Vector2(padding.x, padding.y);
            s_VertScratch[3] = new Vector2(rect.width - padding.z, rect.height - padding.w);

            s_VertScratch[1].x = border.x;
            s_VertScratch[1].y = border.y;
            s_VertScratch[2].x = rect.width - border.z;
            s_VertScratch[2].y = rect.height - border.w;

            for (int i = 0; i < 4; ++i)
            {
                s_VertScratch[i].x += rect.x;
                s_VertScratch[i].y += rect.y;
            }

            s_UVScratch[0] = new Vector2(outer.x, outer.y);
            s_UVScratch[1] = new Vector2(inner.x, inner.y);
            s_UVScratch[2] = new Vector2(inner.z, inner.w);
            s_UVScratch[3] = new Vector2(outer.z, outer.w);

            toFill.Clear();

            for (int x = 0; x < 3; ++x)
            {
                int x2 = x + 1;

                for (int y = 0; y < 3; ++y)
                {
                    if (!fillCenter && x == 1 && y == 1)
                        continue;

                    int y2 = y + 1;

                    AddQuad(toFill,
                        new Vector2(s_VertScratch[x].x, s_VertScratch[y].y),
                        new Vector2(s_VertScratch[x2].x, s_VertScratch[y2].y),
                        color,
                        new Vector2(s_UVScratch[x].x, s_UVScratch[y].y),
                        new Vector2(s_UVScratch[x2].x, s_UVScratch[y2].y),
                        offset);
                }
            }
        }

        /// <summary>
        /// Generate vertices for a filled Image.
        /// </summary>
        static readonly Vector3[] s_Xy = new Vector3[4];

        static readonly Vector3[] s_Uv = new Vector3[4];

        void GenerateFilledSprite(VertexHelper toFill, bool preserveAspect)
        {
            toFill.Clear();

            if (fillAmount < 0.001f)
                return;

            Vector4 v = GetDrawingDimensions(preserveAspect, s_sizeScaling[(int)TextureLayout]);
            Vector2 offset = Vector2.zero;
            Vector4 outer = overrideSprite != null ? GetOuterUV(overrideSprite, m_textureLayout, out offset) : Vector4.zero;

            //UIVertex uiv = UIVertex.simpleVert;
            //uiv.color = color;

            float tx0 = outer.x;
            float ty0 = outer.y;
            float tx1 = outer.z;
            float ty1 = outer.w;

            // Horizontal and vertical filled sprites are simple -- just end the Image prematurely
            if (fillMethod == FillMethod.Horizontal || fillMethod == FillMethod.Vertical)
            {
                if (fillMethod == FillMethod.Horizontal)
                {
                    float fill = (tx1 - tx0) * fillAmount;

                    if (fillOrigin == 1)
                    {
                        v.x = v.z - (v.z - v.x) * fillAmount;
                        tx0 = tx1 - fill;
                    }
                    else
                    {
                        v.z = v.x + (v.z - v.x) * fillAmount;
                        tx1 = tx0 + fill;
                    }
                }
                else if (fillMethod == FillMethod.Vertical)
                {
                    float fill = (ty1 - ty0) * fillAmount;

                    if (fillOrigin == 1)
                    {
                        v.y = v.w - (v.w - v.y) * fillAmount;
                        ty0 = ty1 - fill;
                    }
                    else
                    {
                        v.w = v.y + (v.w - v.y) * fillAmount;
                        ty1 = ty0 + fill;
                    }
                }
            }

            s_Xy[0] = new Vector2(v.x, v.y);
            s_Xy[1] = new Vector2(v.x, v.w);
            s_Xy[2] = new Vector2(v.z, v.w);
            s_Xy[3] = new Vector2(v.z, v.y);

            s_Uv[0] = new Vector2(tx0, ty0);
            s_Uv[1] = new Vector2(tx0, ty1);
            s_Uv[2] = new Vector2(tx1, ty1);
            s_Uv[3] = new Vector2(tx1, ty0);

            {
                if (fillAmount < 1f && fillMethod != FillMethod.Horizontal && fillMethod != FillMethod.Vertical)
                {
                    if (fillMethod == FillMethod.Radial90)
                    {
                        if (RadialCut(s_Xy, s_Uv, fillAmount, fillClockwise, fillOrigin))
                        {
                            int startIndex = toFill.currentVertCount;
                            for (int i = 0; i < 4; i++)
                            {
                                toFill.AddVert(s_Xy[i], color, s_Uv[i], new Vector2(s_Uv[i].x, s_Uv[i].y) + offset, Vector3.zero, Vector4.zero);
                            }
                            toFill.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
                            toFill.AddTriangle(startIndex + 2, startIndex + 3, startIndex);
                        }
                    }
                    else if (fillMethod == FillMethod.Radial180)
                    {
                        for (int side = 0; side < 2; ++side)
                        {
                            float fx0, fx1, fy0, fy1;
                            int even = fillOrigin > 1 ? 1 : 0;

                            if (fillOrigin == 0 || fillOrigin == 2)
                            {
                                fy0 = 0f;
                                fy1 = 1f;
                                if (side == even)
                                {
                                    fx0 = 0f;
                                    fx1 = 0.5f;
                                }
                                else
                                {
                                    fx0 = 0.5f;
                                    fx1 = 1f;
                                }
                            }
                            else
                            {
                                fx0 = 0f;
                                fx1 = 1f;
                                if (side == even)
                                {
                                    fy0 = 0.5f;
                                    fy1 = 1f;
                                }
                                else
                                {
                                    fy0 = 0f;
                                    fy1 = 0.5f;
                                }
                            }

                            s_Xy[0].x = Mathf.Lerp(v.x, v.z, fx0);
                            s_Xy[1].x = s_Xy[0].x;
                            s_Xy[2].x = Mathf.Lerp(v.x, v.z, fx1);
                            s_Xy[3].x = s_Xy[2].x;

                            s_Xy[0].y = Mathf.Lerp(v.y, v.w, fy0);
                            s_Xy[1].y = Mathf.Lerp(v.y, v.w, fy1);
                            s_Xy[2].y = s_Xy[1].y;
                            s_Xy[3].y = s_Xy[0].y;

                            s_Uv[0].x = Mathf.Lerp(tx0, tx1, fx0);
                            s_Uv[1].x = s_Uv[0].x;
                            s_Uv[2].x = Mathf.Lerp(tx0, tx1, fx1);
                            s_Uv[3].x = s_Uv[2].x;

                            s_Uv[0].y = Mathf.Lerp(ty0, ty1, fy0);
                            s_Uv[1].y = Mathf.Lerp(ty0, ty1, fy1);
                            s_Uv[2].y = s_Uv[1].y;
                            s_Uv[3].y = s_Uv[0].y;

                            float val = fillClockwise ? fillAmount * 2f - side : fillAmount * 2f - (1 - side);

                            if (RadialCut(s_Xy, s_Uv, Mathf.Clamp01(val), fillClockwise, ((side + fillOrigin + 3) % 4)))
                            {
                                int startIndex = toFill.currentVertCount;
                                for (int k = 0; k < 4; k++)
                                {
                                    toFill.AddVert(s_Xy[k], color, s_Uv[k], new Vector2(s_Uv[k].x, s_Uv[k].y) + offset, Vector3.zero, Vector4.zero);
                                }
                                toFill.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
                                toFill.AddTriangle(startIndex + 2, startIndex + 3, startIndex);
                            }
                        }
                    }
                    else if (fillMethod == FillMethod.Radial360)
                    {
                        for (int corner = 0; corner < 4; ++corner)
                        {
                            float fx0, fx1, fy0, fy1;

                            if (corner < 2)
                            {
                                fx0 = 0f;
                                fx1 = 0.5f;
                            }
                            else
                            {
                                fx0 = 0.5f;
                                fx1 = 1f;
                            }

                            if (corner == 0 || corner == 3)
                            {
                                fy0 = 0f;
                                fy1 = 0.5f;
                            }
                            else
                            {
                                fy0 = 0.5f;
                                fy1 = 1f;
                            }

                            s_Xy[0].x = Mathf.Lerp(v.x, v.z, fx0);
                            s_Xy[1].x = s_Xy[0].x;
                            s_Xy[2].x = Mathf.Lerp(v.x, v.z, fx1);
                            s_Xy[3].x = s_Xy[2].x;

                            s_Xy[0].y = Mathf.Lerp(v.y, v.w, fy0);
                            s_Xy[1].y = Mathf.Lerp(v.y, v.w, fy1);
                            s_Xy[2].y = s_Xy[1].y;
                            s_Xy[3].y = s_Xy[0].y;

                            s_Uv[0].x = Mathf.Lerp(tx0, tx1, fx0);
                            s_Uv[1].x = s_Uv[0].x;
                            s_Uv[2].x = Mathf.Lerp(tx0, tx1, fx1);
                            s_Uv[3].x = s_Uv[2].x;

                            s_Uv[0].y = Mathf.Lerp(ty0, ty1, fy0);
                            s_Uv[1].y = Mathf.Lerp(ty0, ty1, fy1);
                            s_Uv[2].y = s_Uv[1].y;
                            s_Uv[3].y = s_Uv[0].y;

                            float val = fillClockwise ? fillAmount * 4f - ((corner + fillOrigin) % 4) : fillAmount * 4f - (3 - ((corner + fillOrigin) % 4));

                            if (RadialCut(s_Xy, s_Uv, Mathf.Clamp01(val), fillClockwise, ((corner + 2) % 4)))
                            {
                                int startIndex = toFill.currentVertCount;
                                for (int m = 0; m < 4; m++)
                                {
                                    toFill.AddVert(s_Xy[m], color, s_Uv[m], new Vector2(s_Uv[m].x, s_Uv[m].y) + offset, Vector3.zero, Vector4.zero);
                                }
                                toFill.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
                                toFill.AddTriangle(startIndex + 2, startIndex + 3, startIndex);
                            }
                        }
                    }
                }
                else
                {
                    int startIndex = toFill.currentVertCount;
                    for (int n = 0; n < 4; n++)
                    {
                        toFill.AddVert(s_Xy[n], color, s_Uv[n], new Vector2(s_Uv[n].x, s_Uv[n].y) + offset, Vector3.zero, Vector4.zero);
                    }
                    toFill.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
                    toFill.AddTriangle(startIndex + 2, startIndex + 3, startIndex);
                }
            }
        }

        /// <summary>
        /// Generate vertices for a tiled Image.
        /// </summary>

        void GenerateTiledSprite(VertexHelper toFill)
        {
            Vector2 offset = Vector2.zero;
            Vector4 outer, inner, border;
            Vector2 spriteSize;

            if (overrideSprite != null)
            {
                outer = GetOuterUV(overrideSprite, m_textureLayout, out offset);
                inner = GetInnerUV(overrideSprite, s_sizeScaling[(int)TextureLayout]);
                border = overrideSprite.border;
                spriteSize = overrideSprite.rect.size;

                spriteSize.x *= s_sizeScaling[(int)TextureLayout].x;
                spriteSize.y *= s_sizeScaling[(int)TextureLayout].y;
            }
            else
            {
                outer = Vector4.zero;
                inner = Vector4.zero;
                border = Vector4.zero;
                spriteSize = Vector2.one * 100;
                offset = Vector2.zero;
            }

            Rect rect = GetPixelAdjustedRect();
            float tileWidth = (spriteSize.x - border.x - border.z) / pixelsPerUnit;
            float tileHeight = (spriteSize.y - border.y - border.w) / pixelsPerUnit;
            border = GetAdjustedBorders(border / pixelsPerUnit, rect);

            var uvMin = new Vector2(inner.x, inner.y);
            var uvMax = new Vector2(inner.z, inner.w);

            var v = UIVertex.simpleVert;
            v.color = color;

            // Min to max max range for tiled region in coordinates relative to lower left corner.
            float xMin = border.x;
            float xMax = rect.width - border.z;
            float yMin = border.y;
            float yMax = rect.height - border.w;

            toFill.Clear();
            var clipped = uvMax;

            // if either with is zero we cant tile so just assume it was the full width.
            if (tileWidth == 0)
                tileWidth = xMax - xMin;

            if (tileHeight == 0)
                tileHeight = yMax - yMin;

            if (fillCenter)
            {
                for (float y1 = yMin; y1 < yMax; y1 += tileHeight)
                {
                    float y2 = y1 + tileHeight;
                    if (y2 > yMax)
                    {
                        clipped.y = uvMin.y + (uvMax.y - uvMin.y) * (yMax - y1) / (y2 - y1);
                        y2 = yMax;
                    }

                    clipped.x = uvMax.x;
                    for (float x1 = xMin; x1 < xMax; x1 += tileWidth)
                    {
                        float x2 = x1 + tileWidth;
                        if (x2 > xMax)
                        {
                            clipped.x = uvMin.x + (uvMax.x - uvMin.x) * (xMax - x1) / (x2 - x1);
                            x2 = xMax;
                        }
                        AddQuad(toFill, new Vector2(x1, y1) + rect.position, new Vector2(x2, y2) + rect.position, color, uvMin, clipped, offset);
                    }
                }
            }

            if (hasBorder)
            {
                clipped = uvMax;
                for (float y1 = yMin; y1 < yMax; y1 += tileHeight)
                {
                    float y2 = y1 + tileHeight;
                    if (y2 > yMax)
                    {
                        clipped.y = uvMin.y + (uvMax.y - uvMin.y) * (yMax - y1) / (y2 - y1);
                        y2 = yMax;
                    }
                    AddQuad(toFill,
                        new Vector2(0, y1) + rect.position,
                        new Vector2(xMin, y2) + rect.position,
                        color,
                        new Vector2(outer.x, uvMin.y),
                        new Vector2(uvMin.x, clipped.y),
                        offset);
                    AddQuad(toFill,
                        new Vector2(xMax, y1) + rect.position,
                        new Vector2(rect.width, y2) + rect.position,
                        color,
                        new Vector2(uvMax.x, uvMin.y),
                        new Vector2(outer.z, clipped.y),
                        offset);
                }

                // Bottom and top tiled border
                clipped = uvMax;
                for (float x1 = xMin; x1 < xMax; x1 += tileWidth)
                {
                    float x2 = x1 + tileWidth;
                    if (x2 > xMax)
                    {
                        clipped.x = uvMin.x + (uvMax.x - uvMin.x) * (xMax - x1) / (x2 - x1);
                        x2 = xMax;
                    }
                    AddQuad(toFill,
                        new Vector2(x1, 0) + rect.position,
                        new Vector2(x2, yMin) + rect.position,
                        color,
                        new Vector2(uvMin.x, outer.y),
                        new Vector2(clipped.x, uvMin.y),
                        offset);
                    AddQuad(toFill,
                        new Vector2(x1, yMax) + rect.position,
                        new Vector2(x2, rect.height) + rect.position,
                        color,
                        new Vector2(uvMin.x, uvMax.y),
                        new Vector2(clipped.x, outer.w),
                        offset);
                }

                // Corners
                AddQuad(toFill,
                    new Vector2(0, 0) + rect.position,
                    new Vector2(xMin, yMin) + rect.position,
                    color,
                    new Vector2(outer.x, outer.y),
                    new Vector2(uvMin.x, uvMin.y),
                    offset);
                AddQuad(toFill,
                    new Vector2(xMax, 0) + rect.position,
                    new Vector2(rect.width, yMin) + rect.position,
                    color,
                    new Vector2(uvMax.x, outer.y),
                    new Vector2(outer.z, uvMin.y),
                    offset);
                AddQuad(toFill,
                    new Vector2(0, yMax) + rect.position,
                    new Vector2(xMin, rect.height) + rect.position,
                    color,
                    new Vector2(outer.x, uvMax.y),
                    new Vector2(uvMin.x, outer.w),
                    offset);
                AddQuad(toFill,
                    new Vector2(xMax, yMax) + rect.position,
                    new Vector2(rect.width, rect.height) + rect.position,
                    color,
                    new Vector2(uvMax.x, uvMax.y),
                    new Vector2(outer.z, outer.w),
                    offset);
            }
        }

        static void AddQuad(VertexHelper vertexHelper, Vector2 posMin, Vector2 posMax, Color32 color, Vector2 uvMin, Vector2 uvMax, Vector2 offset)
        {
            int startIndex = vertexHelper.currentVertCount;

            Vector3 zero = Vector3.zero;
            ;
            Vector2 uv0 = new Vector2(uvMin.x, uvMin.y);
            vertexHelper.AddVert(new Vector3(posMin.x, posMin.y, 0), color, uv0, uv0 + offset, zero, zero);

            uv0 = new Vector2(uvMin.x, uvMax.y);
            vertexHelper.AddVert(new Vector3(posMin.x, posMax.y, 0), color, uv0, uv0 + offset, zero, zero);

            uv0 = new Vector2(uvMax.x, uvMax.y);
            vertexHelper.AddVert(new Vector3(posMax.x, posMax.y, 0), color, uv0, uv0 + offset, zero, zero);

            uv0 = new Vector2(uvMax.x, uvMin.y);
            vertexHelper.AddVert(new Vector3(posMax.x, posMin.y, 0), color, uv0, uv0 + offset, zero, zero);

            vertexHelper.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
            vertexHelper.AddTriangle(startIndex + 2, startIndex + 3, startIndex);
        }

        Vector4 GetAdjustedBorders(Vector4 border, Rect rect)
        {
            for (int axis = 0; axis <= 1; axis++)
            {
                // If the rect is smaller than the combined borders, then there's not room for the borders at their normal size.
                // In order to avoid artefacts with overlapping borders, we scale the borders down to fit.
                float combinedBorders = border[axis] + border[axis + 2];
                if (rect.size[axis] < combinedBorders && combinedBorders != 0)
                {
                    float borderScaleRatio = rect.size[axis] / combinedBorders;
                    border[axis] *= borderScaleRatio;
                    border[axis + 2] *= borderScaleRatio;
                }
            }
            return border;
        }

        // Add outerUV
        private static Vector4 GetOuterUV(Sprite sprite, ETextureLayout layout, out Vector2 offset)
        {
            Vector4 outerUV = DataUtility.GetOuterUV(sprite);
            offset = Vector2.zero;
            if (layout != ETextureLayout.Horizonatal)
            {
                offset.y = (outerUV.w - outerUV.y) * 0.5f;
                outerUV.w = (outerUV.w + outerUV.y) * 0.5f;
            }
            else
            {
                offset.x = (outerUV.z - outerUV.x) * 0.5f;
                outerUV.z = (outerUV.z + outerUV.x) * 0.5f;
            }
            return outerUV;
        }

        private static Vector4 GetInnerUV(Sprite sprite, Vector2 sizeScaling)
        {
            Texture texture = sprite.texture;
            if (texture == null)
            {
                return new Vector4(0f, 0f, sizeScaling.x, sizeScaling.y);
            }
            Rect textureRect = sprite.textureRect;
            textureRect.width = textureRect.width * sizeScaling.x;
            textureRect.height = textureRect.height * sizeScaling.y;
            float num = 1f / (float)texture.width;
            float num2 = 1f / (float)texture.height;
            Vector4 padding = DataUtility.GetPadding(sprite);
            Vector4 border = sprite.border;
            float num3 = textureRect.x + padding.x;
            float num4 = textureRect.y + padding.y;
            Vector4 result = default(Vector4);
            result.x = num3 + border.x;
            result.y = num4 + border.y;
            result.z = textureRect.x + textureRect.width - border.z;
            result.w = textureRect.y + textureRect.height - border.w;
            result.x *= num;
            result.y *= num2;
            result.z *= num;
            result.w *= num2;
            return result;
        }

        private static bool RadialCut(Vector3[] xy, Vector3[] uv, float fill, bool invert, int corner)
        {
            if ((double)fill < 0.001)
            {
                return false;
            }
            if ((corner & 1) == 1)
            {
                invert = !invert;
            }
            if (!invert && (double)fill > 0.999000012874603)
            {
                return true;
            }
            float num = Mathf.Clamp01(fill);
            if (invert)
            {
                num = 1f - num;
            }
            float num2 = num * 1.570796f;
            float cos = Mathf.Cos(num2);
            float sin = Mathf.Sin(num2);
            RadialCut(xy, cos, sin, invert, corner);
            RadialCut(uv, cos, sin, invert, corner);
            return true;
        }

        private static void RadialCut(Vector3[] xy, float cos, float sin, bool invert, int corner)
        {
            int num = (corner + 1) % 4;
            int num2 = (corner + 2) % 4;
            int num3 = (corner + 3) % 4;
            if ((corner & 1) == 1)
            {
                if ((double)sin > (double)cos)
                {
                    cos /= sin;
                    sin = 1f;
                    if (invert)
                    {
                        xy[num].x = Mathf.Lerp(xy[corner].x, xy[num2].x, cos);
                        xy[num2].x = xy[num].x;
                    }
                }
                else if ((double)cos > (double)sin)
                {
                    sin /= cos;
                    cos = 1f;
                    if (!invert)
                    {
                        xy[num2].y = Mathf.Lerp(xy[corner].y, xy[num2].y, sin);
                        xy[num3].y = xy[num2].y;
                    }
                }
                else
                {
                    cos = 1f;
                    sin = 1f;
                }
                if (!invert)
                {
                    xy[num3].x = Mathf.Lerp(xy[corner].x, xy[num2].x, cos);
                }
                else
                {
                    xy[num].y = Mathf.Lerp(xy[corner].y, xy[num2].y, sin);
                }
            }
            else
            {
                if ((double)cos > (double)sin)
                {
                    sin /= cos;
                    cos = 1f;
                    if (!invert)
                    {
                        xy[num].y = Mathf.Lerp(xy[corner].y, xy[num2].y, sin);
                        xy[num2].y = xy[num].y;
                    }
                }
                else if ((double)sin > (double)cos)
                {
                    cos /= sin;
                    sin = 1f;
                    if (invert)
                    {
                        xy[num2].x = Mathf.Lerp(xy[corner].x, xy[num2].x, cos);
                        xy[num3].x = xy[num2].x;
                    }
                }
                else
                {
                    cos = 1f;
                    sin = 1f;
                }
                if (invert)
                {
                    xy[num3].y = Mathf.Lerp(xy[corner].y, xy[num2].y, sin);
                }
                else
                {
                    xy[num].x = Mathf.Lerp(xy[corner].x, xy[num2].x, cos);
                }
            }
        }
    }
}
