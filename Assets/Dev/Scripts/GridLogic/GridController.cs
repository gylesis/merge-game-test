using System;
using System.Collections.Generic;
using System.Linq;
using Dev.UnitsLogic;
using UniRx;
using Unity.Mathematics;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Dev.GridLogic
{
    public class GridController : MonoBehaviour
    {
        [SerializeField] private GridCellView _gridCellViewPrefab;
        [SerializeField] private Vector2Int _gridSize;
        [SerializeField] private Transform _gridStartPoint;
        [SerializeField] private Vector3 _cellSize = Vector3.one;

        [Min(1)] [SerializeField] private float _offset = 1.12f;

        private CellData[,] _cells;
        private List<Vector2Int> _busyCells;
        private List<GridCellView> _gridCellViews;

        public int BusyCellsCount => _busyCells.Count;
        public int MaxUnitLevel { get; private set; }
        public Vector2Int GridSize => _gridSize;

        private UnitsStaticDataContainer _unitsStaticDataContainer;

        [Inject]
        private void Construct(UnitsStaticDataContainer unitsStaticDataContainer)
        {
            _unitsStaticDataContainer = unitsStaticDataContainer;
        }

        private void Start()
        {
            SetupGrid();
        }

        private void SetupGrid()
        {
            MaxUnitLevel = 1;
            _cells = new CellData[_gridSize.x, _gridSize.y];
            _busyCells = new List<Vector2Int>();
            _gridCellViews = new List<GridCellView>();
            
            Vector3 spawnPos = _gridStartPoint.position;
            Vector3 originPos = spawnPos;

            for (int i = 0; i < _gridSize.x; i++)
            {
                int x = i;

                for (int j = 0; j < _gridSize.y; j++)
                {
                    int y = j;

                    Vector2Int index = new Vector2Int(x, y);

                    spawnPos = originPos + Vector3.right * (x * _offset + _cellSize.x) +
                               Vector3.forward * (y * _offset + _cellSize.z);

                    GridCellView cellView = Instantiate(_gridCellViewPrefab, spawnPos, quaternion.identity);

                    cellView.CellDataIndex = index;
                    cellView.transform.localScale = _cellSize;

                    _gridCellViews.Add(cellView);
                    
                    CellData cellData = new CellData(index, null);
                    _cells[x, y] = cellData;

                    /*if (Random.value > 0.8)
                    {
                        SpawnUnit(cellView, Extensions.MergeGame.GetRandomUnitType(), 1);
                    }*/
                }
            }
        }

        public bool TryPlaceUnit(GridUnit unit, GridCellView targetCellView)
        {
            CellData targetCellData = GetCellData(targetCellView);

            if (targetCellData.IsFree == false)
            {
                if (targetCellData.UnitData.UnitID == unit.Id)
                {
                    Debug.Log($"Same cell");
                    ReturnUnitBackToCell(unit);
                    return false;
                }
                
                CellData unitCell = GetUnitCell(unit);
                int targetUnitLevel = targetCellData.UnitData.Level;
                int originUnitLevel = unitCell.UnitData.Level;
                
                UnitType targetUnitType = targetCellData.UnitData.UnitType;
                UnitType originUnitType = unitCell.UnitData.UnitType;

                if (targetUnitLevel == originUnitLevel)
                {
                    if (targetUnitType == originUnitType)
                    {
                        MergeUnits(unit, targetCellData);
                        Debug.Log($"Merged units level of {targetUnitLevel}");
                        return true;
                    }
                    else
                    {
                        ReturnUnitBackToCell(unit);
                        Debug.Log($"Cant't place units, unit types are different");
                        return false;
                    }
                }
                else
                {
                    ReturnUnitBackToCell(unit);
                    Debug.Log($"Cant't place units, levels are different");
                    return false;
                }
            }
            else
            {
                PlaceUnitOnCellView(unit, targetCellView);
                return true;
            }
           
        }

        private void MergeUnits(GridUnit firstUnit, CellData targetCellUnit)
        {
            CellData cellData = GetUnitCell(firstUnit);

            int nextLevel = cellData.UnitData.Level + 1;
            UnitType unitType = cellData.UnitData.UnitType;

            if (nextLevel > MaxUnitLevel)
            {
                MaxUnitLevel = nextLevel;
            }
            
            RemoveUnitFromCell(firstUnit);
            RemoveUnitFromCell(targetCellUnit.UnitData.GridUnit);

            SpawnUnit(GetCellView(targetCellUnit), unitType, nextLevel, true);
        }

        private void RemoveUnitFromCell(GridUnit gridUnit)
        {
            CellData unitCell = GetUnitCell(gridUnit);
            unitCell.AssignUnitData(null);

            _busyCells.Remove(unitCell.Index);
            gridUnit.OnDestroy.OnNext(Unit.Default);
            
            Destroy(gridUnit.gameObject);
        }
        
        public bool TrySpawnUnit(GridCellView gridCellView, UnitType unitType, int level)
        {
            CellData cellData = GetCellData(gridCellView);

            bool cellHasUnit = cellData.IsFree == false;
            
            if (cellHasUnit)
            {   
                if (cellData.UnitData.Level == level)
                {
                    UpgradeUnit(cellData.UnitData);
                    return true;
                }

                return false;
            }
            else
            {
                return SpawnUnit(gridCellView, unitType, level);
            }
            
        }

        private bool SpawnUnit(GridCellView gridCellView, UnitType unitType, int level, bool afterMerge = false)
        {   
            bool hasData = _unitsStaticDataContainer.TryGetData(unitType, out UnitStaticData unitStaticData);
    
            if (hasData == false)   
            {
                Debug.Log($"Couldn't spawn unit type of {unitType}, no static data presented");
                return false;
            }
            
            GridUnit gridUnit = Instantiate(unitStaticData.UnitPrefab, gridCellView.PlacePos, Quaternion.identity);
            gridUnit.OnLevelUp(level, afterMerge);
            
            CellData cellData = GetCellData(gridCellView);
            GridUnitData gridUnitData = new GridUnitData(gridUnit, level);
            cellData.AssignUnitData(gridUnitData);
            _busyCells.Add(cellData.Index);
            
            return true;
        }

        public void ReturnUnitBackToCell(GridUnit gridUnit)
        {
            IterateThroughCells(OnIterate);

            void OnIterate(int x, int y)
            {
                CellData cellData = GetCell(x, y);

                if (cellData.IsFree == false) 
                {
                    if (cellData.UnitData.AreIDsMatchingWithUnit(gridUnit))
                    {
                        GridCellView cellView = GetCellView(cellData);

                        PlaceUnitOnCellView(gridUnit, cellView);
                    }
                }
            }
        }

        private CellData GetUnitCell(GridUnit gridUnit)
        {
            for (int i = 0; i < _gridSize.x; i++)
            {
                int x = i;

                for (int j = 0; j < _gridSize.y; j++)
                {
                    int y = j;

                    CellData cellData = GetCell(x, y);

                    if(cellData.IsFree) continue;
                    
                    if (cellData.UnitData.UnitID == gridUnit.Id)
                    {
                        return cellData;
                    }
                }
            }

            return null;
        }

        private void PlaceUnitOnCellView(GridUnit unit, GridCellView targetCellView)
        {
            CellData cellData = GetUnitCell(unit);
            _busyCells.Remove(cellData.Index);
            
            GridUnitData unitData = cellData.UnitData;
            cellData.AssignUnitData(null);

            unit.transform.position = targetCellView.PlacePos;

            CellData targetCellData = GetCellData(targetCellView);
            targetCellData.AssignUnitData(unitData);
            
            _busyCells.Add(targetCellData.Index);
        }
        
        private void UpgradeUnit(GridUnitData gridUnitData)
        {
            gridUnitData.Level++;
            gridUnitData.GridUnit.OnLevelUp(gridUnitData.Level, true);
        }

        private void IterateThroughCells(Action<int, int> onIterate)
        {
            for (int i = 0; i < _gridSize.x; i++)
            {
                int x = i;

                for (int j = 0; j < _gridSize.y; j++)
                {
                    int y = j;

                    onIterate?.Invoke(x, y);
                }
            }
        }

        public bool IsCellViewBusy(GridCellView gridCellView)
        {
            return GetCellData(gridCellView).IsFree == false;
        }

        public GridUnit SpawnCopyOfUnit(GridCellView gridCellView)
        {
            GridUnit gridUnit = GetCellData(gridCellView).UnitData.GridUnit;
            GridUnit newUnit = Instantiate(gridUnit);
            newUnit.SetColor(gridUnit.CurrentColor);

            Destroy(newUnit.GetComponent<GroundDraggableObject>());
            
            return newUnit;
        }

        public Vector3 GetHighestCellPos()
        {
            return GetCellView(GetCell(_gridSize.x - 1, _gridSize.y / 2)).PlacePos;
        }
        
        public void RemoveUnitFromCell(GridCellView gridCellView)
        {
            CellData cellData = GetCellData(gridCellView);

            RemoveUnitFromCell(cellData.UnitData.GridUnit);
        }
 
        public bool TryGetRandomBusyCell(out GridCellView cellView)
        {
            cellView = null;
            
            if (_busyCells.Count == 0)
            {
                return false;
            }
            
            Vector2Int randomCellIndex = _busyCells[Random.Range(0, _busyCells.Count)];

            CellData cellData = GetCell(randomCellIndex.x, randomCellIndex.y);
            cellView = GetCellView(cellData);

            return true;
        }
        
        private CellData GetCell(int x, int y)
        {
            return _cells[x, y];
        }

        private GridCellView GetCellView(CellData cellData)
        {
            return _gridCellViews.First(x => x.CellDataIndex == cellData.Index);
        }

        private CellData GetCellData(GridCellView gridCellView)
        {
            return _cells[gridCellView.CellDataIndex.x, gridCellView.CellDataIndex.y];
        }
        
        private void OnDrawGizmos()
        {
            Vector3 spawnPos = _gridStartPoint.position;

            for (int x = 0; x < _gridSize.x; x++)
            {
                for (int y = 0; y < _gridSize.y; y++)
                {
                    spawnPos = _gridStartPoint.position + Vector3.right * (x * _offset + _cellSize.x) + Vector3.forward *
                        (y * _offset + _cellSize.z);

                    Gizmos.DrawWireCube(spawnPos, _cellSize);
                }
            }
        }
    }
}