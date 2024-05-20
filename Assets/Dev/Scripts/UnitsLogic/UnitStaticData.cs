using UnityEngine;

namespace Dev.UnitsLogic
{
    [CreateAssetMenu(menuName = "StaticData/UnitStaticData", fileName = "UnitStaticData", order = 0)]
    public class UnitStaticData : ScriptableObject
    {
        public GridUnit UnitPrefab;
        public UnitType UnitType;
    }
}