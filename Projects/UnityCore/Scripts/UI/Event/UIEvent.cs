using UnityEngine;
using UnityEngine.EventSystems;

namespace MonMoose.Core
{
    public class UIEvent : ClassPoolObj
    {
        public GameObject widget;
        public PointerEventData eventData;
        public object extraParam;


        public override void OnRelease()
        {
            widget = null;
            extraParam = null;
            eventData = null;
        }
    }
}

