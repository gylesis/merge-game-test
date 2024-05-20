using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dev.UnitsLogic
{
    [RequireComponent(typeof(RectTransform))]
    public class UIDraggableObject : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [SerializeField] private RectTransform _rectTransform;

        public RectTransform RectTransform => _rectTransform;

        private void Reset()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public Subject<PointerEventData> PointerDown { get; } = new Subject<PointerEventData>();
        public Subject<PointerEventData> PointerUp { get; } = new Subject<PointerEventData>();
        public Subject<PointerEventData> PointerDrag { get; } = new Subject<PointerEventData>();
            
        
        public void OnDrag(PointerEventData eventData)
        {
            PointerDrag.OnNext(eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            PointerDown.OnNext(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            PointerUp.OnNext(eventData);
        }
    }

}