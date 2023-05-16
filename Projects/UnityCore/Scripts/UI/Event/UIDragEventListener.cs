using UnityEngine;
using UnityEngine.EventSystems;

namespace MonMoose.Core
{
    public class UIDragEventListener : UIEventListener, IDragHandler
    {
        public new static UIDragEventListener Get(GameObject go)
        {
            UIDragEventListener eventListener = go.GetComponent<UIDragEventListener>();
            if (eventListener == null)
            {
                eventListener = go.AddComponent<UIDragEventListener>();
            }
            return eventListener;
        }

        public void OnDrag(PointerEventData eventData)
        {
            HandleEvent((int)UIEventType.Drag, eventData);
        }
    }
}

