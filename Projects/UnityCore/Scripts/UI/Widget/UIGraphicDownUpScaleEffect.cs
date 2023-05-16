using UnityEngine;
using UnityEngine.EventSystems;

namespace MonMoose.Core
{
    public class UIGraphicDownUpScaleEffect : UIGraphicDownUpEffect
    {
        public float scale = 0.9f;
        public float time = 0.1f;

        private Vector3 m_initScale;
        private StaticLerpVec3 m_scaleLerp = new StaticLerpVec3();

        protected override void Awake()
        {
            base.Awake();
            m_initScale = transform.localScale;
            m_scaleLerp.actionOnTick = OnScaleLerpTick;
        }

        protected override void OnDisable()
        {
            m_scaleLerp.Stop();
            transform.localScale = m_initScale;
        }

        private void OnScaleLerpTick(AbstractStaticLerp<Vector3> lerp)
        {
            transform.localScale = m_scaleLerp.curValue;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            m_scaleLerp.Reset();
            m_scaleLerp.Start(m_initScale, m_initScale * scale, time, EBaseLerpFuncType.Linear);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            m_scaleLerp.Reset();
            m_scaleLerp.Start(m_initScale * scale, m_initScale, time, EBaseLerpFuncType.Linear);
        }

        protected virtual void Update()
        {
            m_scaleLerp.Tick(Time.deltaTime);
        }
    }
}
