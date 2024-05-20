using Dev.UnitsLogic;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace Dev.GridLogic
{
    public class UfoService : MonoBehaviour
    {
        [SerializeField] private UFO _ufo;
        [SerializeField] private float _ufoHeight = 5;
        [SerializeField] private float _ufoHuntBaseTime = 10;

        [SerializeField] private float _ufoCaptureUnitTime = 3;
        
        [SerializeField] private AnimationCurve _ufoHuntTimeDifficulty;
        
        private GridController _gridController;
        private GridCellView _currentCellView;

        private float _ufoHuntTimer; 
        
        private void Awake()
        {
            SetUFOActiveState(false);
        }

        [Inject]
        private void Construct(GridController gridController)
        {
            _gridController = gridController;
        }

        private void Update()
        {
            _ufoHuntTimer += Time.deltaTime;
            
            int cellsCount = _gridController.GridSize.x * _gridController.GridSize.y;

            float time = 1 - ((float)(_gridController.MaxUnitLevel * _gridController.BusyCellsCount) / cellsCount);
            float modifier = _ufoHuntTimeDifficulty.Evaluate(time);
            float huntTime = modifier * _ufoHuntBaseTime;   

            if (_ufoHuntTimer >= huntTime)
            {
                _ufoHuntTimer = 0;
                
                CallUFO();
            }
            
            
            if (Input.GetKeyDown(KeyCode.G)) // mb cause problems
            {
                CallUFO();
            }
        }

        private void CallUFO()
        {
            if (_currentCellView != null)
            {
                _currentCellView.SetBlinkingState(false);
            }

            if (_gridController.TryGetRandomBusyCell(out var cellView) == false) return;

            _currentCellView = cellView;
            _currentCellView.SetBlinkingState(true);

            SetUFOActiveState(true);

            Vector3 ufoPos = _gridController.GetHighestCellPos() + Vector3.up * _ufoHeight;
            _ufo.transform.position = ufoPos;

            Vector3 targetUfoPos = _currentCellView.PlacePos + Vector3.up * _ufoHeight;
            _ufo.transform.DOMove(targetUfoPos, 1.5f);

            GridUnit copyOfUnit = _gridController.SpawnCopyOfUnit(_currentCellView);
            copyOfUnit.gameObject.SetActive(false);
    
            bool hasPlayerRemovedUnit = false;
            
            Sequence sequence = DOTween.Sequence();

            sequence    
                .AppendInterval(_ufoCaptureUnitTime)
                .AppendCallback((() =>
                {
                    bool isCellStillBusy = _gridController.IsCellViewBusy(_currentCellView);
                    
                    if (isCellStillBusy)
                    {
                        hasPlayerRemovedUnit = true;
                        copyOfUnit.gameObject.SetActive(true);
                        copyOfUnit.transform.DOMoveY(_ufo.transform.position.y - 1, 1);
                        _gridController.RemoveUnitFromCell(_currentCellView);
                    }
                  
                }))
                .AppendInterval(1)
                .AppendCallback((() =>
                {
                    Destroy(copyOfUnit.gameObject);
                    
                    ReturnUFO();
                    _currentCellView.SetBlinkingState(false);
                }));

        }

        private void ReturnUFO()
        {
            _ufo.transform.DOMove(new Vector3(99, _ufo.transform.position.y, 99), 0.5f).OnComplete((() =>
            {
                SetUFOActiveState(false);
            }));
        }

        private void SetUFOActiveState(bool isActive)
        {
            _ufo.gameObject.SetActive(isActive);
        }
    }
}