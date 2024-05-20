namespace Dev.UnitsLogic
{
    public class SquareUnit : GridUnit
    {
        public override UnitType UnitType => UnitType.Square;

        public override void OnLevelUp(int level, bool afterMerge)
        {
            base.OnLevelUp(level, afterMerge);
            
            if (afterMerge == false) return;
            
            SpawnFX("unit_square_upgrade");
        }
    }
}