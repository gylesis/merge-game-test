using Dev.Infrastructure;
using Dev.UnitsLogic;
using UnityEngine;
using Zenject;

namespace Dev.GridLogic
{
    public class GroundObjectsDragHandler : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private float _spherecastRadius = 0.05f;
        [SerializeField] private float _dragObjHeight = 2f;
        [SerializeField] private float _objectFollowCursorSpeed = 50;
        [SerializeField] private Transform _ground;
        [SerializeField] private LayerMask _gridCellViewLayer;
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private LayerMask _draggableObjectsLayerMask;

        private float _dragHeight;
        private bool _isActive = true;

        private GroundDraggableObject _focusedDragObj;
        private bool _isDragging;
        private Vector3 _originObjPosition;

        private InputService _inputService;
        private GridController _gridController;
        private UnitsCardsService _unitsCardsService;

        [Inject]
        private void Construct(InputService inputService, GridController gridController, UnitsCardsService unitsCardsService)
        {
            _unitsCardsService = unitsCardsService;
            _gridController = gridController;
            _inputService = inputService;
        }
        
        private void Update()
        {
            if(_isActive == false) return;
            
            if(_unitsCardsService.IsDraggingCard) return;
            
            DragHandle();

            if(_isDragging)
            {
                if (_focusedDragObj == null) _isDragging = false;
                return;
            }

            PollForNewObjectsToDrag();
        }

        private void PollForNewObjectsToDrag()
        {
            Ray ray = _camera.ScreenPointToRay(_inputService.PointerPos);

            bool sphereCast = Physics.SphereCast(ray, _spherecastRadius, out var hit, float.MaxValue, _draggableObjectsLayerMask);

            if (sphereCast)
            {
                bool isDraggableObject = hit.collider.TryGetComponent<GroundDraggableObject>(out var draggableObject);
                
                if (isDraggableObject)
                {
                    _focusedDragObj = draggableObject;
                }
                else
                {
                    _focusedDragObj = null;
                }
            }
            else
            {
                _focusedDragObj = null;
            }
        }
        
        private void DragHandle()
        {
            if(_focusedDragObj == null) return;
            
            if (_inputService.IsPointerDown)
            {
                OnObjectStartedToDrag();
            }

            if (_inputService.IsPointerDrag)
            {
                OnObjectDrag();
            }

            if (_inputService.IsPointerUp)
            {
                OnObjectFinishedDrag();
            }
        }

        private void OnObjectStartedToDrag()
        {
            _isDragging = true;
                
            _dragHeight = _ground.transform.position.y + _dragObjHeight;
        }

        private void OnObjectDrag()
        {
            Ray ray = _camera.ScreenPointToRay(_inputService.PointerPos);
            var raycast = Physics.Raycast(ray, out var hit, float.MaxValue, _groundLayer);
   
            _isDragging = true;

            Vector3 pos = hit.point;
            pos.y = _dragHeight;
                
            Vector3 position = Vector3.Lerp(_focusedDragObj.transform.position, pos, Time.deltaTime * _objectFollowCursorSpeed);
            _focusedDragObj.transform.position = position;
        }

        private void OnObjectFinishedDrag()
        {
            bool isGridUnit = _focusedDragObj.TryGetComponent<GridUnit>(out var gridUnit);

            if (isGridUnit)
            {
                Ray ray = _camera.ScreenPointToRay(_inputService.PointerPos);

                bool sphereCast =
                    Physics.SphereCast(ray, _spherecastRadius, out var hit, float.MaxValue, _gridCellViewLayer);

                if (sphereCast)
                {
                    bool isGridCell = hit.collider.TryGetComponent<GridCellView>(out var cellView);

                    if (isGridCell)
                    {
                        _gridController.TryPlaceUnit(gridUnit, cellView);
                    }
                    else
                    {
                        _gridController.ReturnUnitBackToCell(gridUnit);
                    }
                }
                else
                {
                    _gridController.ReturnUnitBackToCell(gridUnit);
                }
               
            }

            _isDragging = false;
            _focusedDragObj = null;
        }

    }
}