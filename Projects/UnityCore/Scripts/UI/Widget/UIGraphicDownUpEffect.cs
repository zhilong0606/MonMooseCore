using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MonMoose.Core
{
    public abstract class UIGraphicDownUpEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private Button m_button;

        protected virtual bool effectEnable
        {
            get { return !isActiveAndEnabled || m_button == null || m_button.isActiveAndEnabled || m_button.interactable; }
        }

        protected virtual void Awake()
        {
            m_button = GetComponent<Button>();
        }

        protected virtual void OnDisable()
        {

        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
        }
    }
}
