using Dev.UnitsLogic;
using UnityEngine;

namespace Dev.GridLogic
{
    public class CellData
    {
        public Vector2Int Index { get; private set; }
        public GridUnitData UnitData { get; private set; }
        public bool IsFree => UnitData == null;
        
        public CellData(Vector2Int index, GridUnitData unitData)
        {   
            Index = index;
            UnitData = unitData;
        }

        public void AssignUnitData(GridUnitData unit)
        {
            UnitData = unit;
        }
    }
}