using UnityEngine.EventSystems;

namespace Dev.UnitsLogic
{
    public struct PointerEventContext
    {
        public UIDraggableObject DraggableObject;
        public PointerEvent PointerEventType;
        public PointerEventData EventData;
    }
}