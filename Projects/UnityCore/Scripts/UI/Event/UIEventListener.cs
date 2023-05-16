using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MonMoose.Core
{
    public class UIEventListener : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        protected UIEvent[] eventInfos = new UIEvent[(int)UIEventType.Count];
        protected Action<UIEvent>[] callbacks = new Action<UIEvent>[(int)UIEventType.Count];

        public static UIEventListener Get(GameObject go)
        {
            Graphic graphic = go.GetComponent<Graphic>();
            if (graphic != null)
            {
                graphic.raycastTarget = true;
            }
            UIEventListener eventListener = go.GetComponent<UIEventListener>();
            if (eventListener == null)
            {
                eventListener = go.AddComponent<UIEventListener>();
            }
            return eventListener;
        }

        public UIEvent GetEvent(int eventType)
        {
            UIEvent e = eventInfos[eventType];
            if (e == null)
            {
                e = ClassPoolManager.instance.Fetch<UIEvent>();
                eventInfos[eventType] = e;
            }
            return e;
        }

        protected void HandleEvent(int eventType, PointerEventData eventData)
        {
            UIEvent e = eventInfos[eventType];
            if (e != null)
            {
                e.eventData = eventData;
                if (callbacks[eventType] != null)
                {
                    callbacks[eventType](e);
                }
                e.eventData = null;
            }
        }

        public void SetEvent(UIEventType eventType, Action<UIEvent> callback)
        {
            UIEvent e = GetEvent((int)eventType);
            e.widget = gameObject;
            callbacks[(int)eventType] = callback;
        }

        public void SetEvent(UIEventType eventType, Action<UIEvent> callback, object extraParam)
        {
            UIEvent e = GetEvent((int)eventType);
            e.widget = gameObject;
            e.extraParam = extraParam;
            callbacks[(int)eventType] = callback;
        }

        public void SetParam(UIEventType eventType, object extraParam)
        {
            UIEvent e = eventInfos[(int)eventType];
            if (e != null)
            {
                e.extraParam = extraParam;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            HandleEvent((int)UIEventType.PointerClick, eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            eventData.useDragThreshold = false;
            HandleEvent((int)UIEventType.PointerDown, eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            HandleEvent((int)UIEventType.PointerUp, eventData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            HandleEvent((int)UIEventType.PointerEnter, eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            HandleEvent((int)UIEventType.PointerExit, eventData);
        }

        private void OnDestroy()
        {
            for (int i = 0; i < (int)UIEventType.Count; ++i)
            {
                if (eventInfos[i] != null)
                {
                    eventInfos[i].Release();
                    eventInfos[i] = null;
                }
                callbacks[i] = null;
            }
        }
    }
}

