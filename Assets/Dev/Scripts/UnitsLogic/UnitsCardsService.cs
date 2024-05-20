using System;
using System.Collections.Generic;
using System.Linq;
using Dev.GridLogic;
using Dev.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
using Random = UnityEngine.Random;

namespace Dev.UnitsLogic
{
    public class UnitsCardsService : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private float _spherecastRadius = 0.05f;
        [SerializeField] private Transform _ground;

        [SerializeField] private UnitCard _unitCardPrefab;
        [SerializeField] private List<UnitCard> _unitCards;

        [SerializeField] private Transform _unitsCardsParent;

        [SerializeField] private LayerMask _gridCellsLayerMask;

        [SerializeField] private int _newCardsSpawnRateInSecs = 7;
        
        private GridController _gridController = null;
        private UIDraggableObject _draggableObject = null;
        private Vector3 _originPos = default;

        public bool IsDraggingCard { get; private set; }

        [Inject]
        private void Construct(GridController gridController)
        {
            _gridController = gridController;
        }
        
        private void Start()
        {
            int level = 1;
            UnitType unitType = Extensions.MergeGame.GetRandomUnitType();
                    
            SpawnCard(level, unitType);
            
            Observable
                .Interval(TimeSpan.FromSeconds(_newCardsSpawnRateInSecs))
                .TakeUntilDestroy(this)
                .Subscribe((l =>
                {
                    int level = 1;
                    UnitType unitType = Extensions.MergeGame.GetRandomUnitType();
                    
                    SpawnCard(level, unitType);
                }));
        }

        private void SpawnCard(int level, UnitType unitType)
        {
            UnitCard unitCard = Instantiate(_unitCardPrefab, _unitsCardsParent);
            unitCard.Setup(level, unitType);

            _unitCards.Add(unitCard);
            RegisterUIDraggableObject(unitCard.UIDraggableObject);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                SpawnCard(1, Extensions.MergeGame.GetRandomUnitType());
            }
        }

        public void RegisterUIDraggableObject(UIDraggableObject uiDraggableObject)
        {
            uiDraggableObject.PointerDown.TakeUntilDestroy(this)
                .Subscribe((data => OnPointerDown(data, uiDraggableObject)));
            uiDraggableObject.PointerUp.TakeUntilDestroy(this)
                .Subscribe((data => OnPointerUp(data, uiDraggableObject)));
            uiDraggableObject.PointerDrag.TakeUntilDestroy(this)
                .Subscribe((data => OnPointerDrag(data, uiDraggableObject)));
        }

        private void OnPointerDown(PointerEventData eventData, UIDraggableObject draggableObject)
        {
            IsDraggingCard = true;

            _originPos = draggableObject.RectTransform.position;
            _draggableObject = draggableObject;
        }

        private void OnPointerDrag(PointerEventData eventData, UIDraggableObject draggableObject)
        {
            draggableObject.RectTransform.position = eventData.position;
        }

        private void OnPointerUp(PointerEventData eventData, UIDraggableObject draggableObject)
        {
            IsDraggingCard = false;
            Ray ray = _camera.ScreenPointToRay(eventData.position);

            bool sphereCast =
                Physics.SphereCast(ray, _spherecastRadius, out var hit, float.MaxValue, _gridCellsLayerMask);

            if (sphereCast)
            {
                bool isGridCell = hit.collider.TryGetComponent<GridCellView>(out var gridCellView);

                if (isGridCell)
                {
                    UnitCard unitCard = draggableObject.GetComponent<UnitCard>();

                    if (_gridController.TrySpawnUnit(gridCellView, unitCard.UnitType, unitCard.UnitLevel))
                    {
                        Remove(draggableObject);
                        return;
                    }
                }
            }

            ReturnToOriginPos(draggableObject);
        }

        private void Remove(UIDraggableObject uiDraggableObject)
        {
            Destroy(uiDraggableObject.gameObject);
        }

        private void ReturnToOriginPos(UIDraggableObject draggableObject)
        {
            draggableObject.RectTransform.position = _originPos;
        }
    }
}