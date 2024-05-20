namespace Dev.UnitsLogic
{
    public class SphereUnit : GridUnit
    {
        public override UnitType UnitType => UnitType.Sphere;

        public override void OnLevelUp(int level, bool afterMerge)
        {
            base.OnLevelUp(level, afterMerge);

            if (afterMerge == false) return;
            
            SpawnFX("unit_sphere_upgrade");
        }
    }
}