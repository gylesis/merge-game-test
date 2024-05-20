namespace Dev.UnitsLogic
{
    public class CapsuleUnit : GridUnit
    {
        public override UnitType UnitType => UnitType.Capsule;

        public override void OnLevelUp(int level, bool afterMerge)
        {
            base.OnLevelUp(level, afterMerge);

            if (afterMerge == false) return;
            
            SpawnFX("unit_capsule_upgrade");
        }
    }
}