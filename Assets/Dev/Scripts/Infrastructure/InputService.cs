using UnityEngine;
using Zenject;

namespace Dev.Infrastructure
{
    public class InputService : ITickable
    {
#if UNITY_EDITOR

        public bool IsPointerDown => Input.GetMouseButtonDown(0);
        public bool IsPointerUp => Input.GetMouseButtonUp(0);
        public bool IsPointerDrag => Input.GetMouseButton(0);

#else
        public bool IsPointerDown => _isPointerDown;
        public bool IsPointerUp => _isPointerUp;
        public bool IsPointerDrag => _isPointerDrag;
#endif

        public Vector2 PointerPos { get; private set; }

        private bool _isPointerUp = false;
        private bool _isPointerDown = false;
        private bool _isPointerDrag = false;

        private bool _hasTouch;
        
        public void Tick()
        {
            _isPointerUp = false;

            if (Input.touchCount > 0)
            {
                if (_hasTouch == false)
                {
                    _isPointerDown = true;
                }

                _isPointerDrag = true;
                _hasTouch = true;

                PointerPos = Input.touches[0].position;
            }
            else
            {
                _isPointerDrag = false;

                if (_hasTouch)
                {
                    _isPointerUp = true;
                }

                _hasTouch = false;
                PointerPos = Input.mousePosition;
            }
        }
        
    }
}