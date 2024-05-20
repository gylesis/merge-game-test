using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dev.UnitsLogic
{
    [CreateAssetMenu(menuName = "StaticData/UnitsStaticDataContainer", fileName = "UnitsStaticDataContainer", order = 0)]
    public class UnitsStaticDataContainer : ScriptableObject
    {
        [SerializeField] private List<UnitStaticData> _unitStaticDatas;

        public bool TryGetData(UnitType unitType, out UnitStaticData data)
        {
            UnitStaticData staticData = _unitStaticDatas.FirstOrDefault(x => x.UnitType == unitType);

            data = staticData;
            
            if (staticData == null)
            {
                return false;
            }

            return true;
        }
    }
}