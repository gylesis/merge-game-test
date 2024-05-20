namespace Dev.UnitsLogic
{
    public class GridUnitData
    {
        public GridUnit GridUnit { get; set; }
        public int Level { get; set; }
        public int UnitID { get; private set; }

        public UnitType UnitType => GridUnit.UnitType;
        
        public GridUnitData(GridUnit gridUnit, int level)
        {
            UnitID = gridUnit.gameObject.GetInstanceID();
            GridUnit = gridUnit;
            Level = level;
        }

        public bool AreIDsMatchingWithUnit(GridUnit unit)
        {
            return UnitID == unit.gameObject.GetInstanceID();
        }
            
    }
}