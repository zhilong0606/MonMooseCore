using UnityEngine;
using UnityEngine.UI;

namespace MonMoose.Core
{
    public class UIGradient : BaseMeshEffect
    {
        [SerializeField] private Color m_color1 = Color.white;
        [SerializeField] private Color m_color2 = Color.black;
        [SerializeField] private float m_dir = 0f;
        [SerializeField] private float m_startPos = 0f;
        [SerializeField] private float m_endPos = 1f;

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!isActiveAndEnabled)
                return;
            UIVertex uiVertex = UIVertex.simpleVert;
            float minX = float.MaxValue;
            float maxX = float.MinValue;
            float minY = float.MaxValue;
            float maxY = float.MinValue;

            for (int i = 0; i < vh.currentVertCount; i++)
            {
                vh.PopulateUIVertex(ref uiVertex, i);
                minX = Mathf.Min(minX, uiVertex.position.x);
                maxX = Mathf.Max(maxX, uiVertex.position.x);
                minY = Mathf.Min(minY, uiVertex.position.y);
                maxY = Mathf.Max(maxY, uiVertex.position.y);
            }

            float sin = Mathf.Abs(Mathf.Sin(m_dir * Mathf.Deg2Rad));
            float cos = Mathf.Cos(m_dir * Mathf.Deg2Rad);
            float width = Mathf.Max(0f, maxX - minX);
            float height = Mathf.Max(0f, maxY - minY);

            if (width > 0f && height > 0f)
            {
                for (int i = 0; i < vh.currentVertCount; i++)
                {
                    vh.PopulateUIVertex(ref uiVertex, i);
                    float x = uiVertex.position.x - minX;
                    float y = uiVertex.position.y - minY;
                    if (m_dir < 0f)
                    {
                        x = width - x;
                    }
                    float f = (x * sin + y * cos) / (width * sin + height * cos);
                    f = Mathf.Clamp01(Mathf.InverseLerp(m_startPos, m_endPos, f));
                    uiVertex.color *= Color.Lerp(m_color2, m_color1, f);
                    vh.SetUIVertex(uiVertex, i);
                }
            }
        }
    }
}
