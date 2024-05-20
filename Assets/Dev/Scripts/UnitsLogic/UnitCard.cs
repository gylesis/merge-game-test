using TMPro;
using UnityEngine;

namespace Dev.UnitsLogic
{
    [RequireComponent(typeof(UIDraggableObject))]
    public class UnitCard : MonoBehaviour
    {
        [SerializeField] private TMP_Text _infoText;
        [SerializeField] private UIDraggableObject _uiDraggableObject;
       
        public UnitType UnitType { get; private set; }
        public int UnitLevel { get; private set; }
        public UIDraggableObject UIDraggableObject => _uiDraggableObject;

        private void Reset()
        {
            _uiDraggableObject = GetComponent<UIDraggableObject>();
        }
    
        public void Setup(int unitLevel, UnitType unitType)
        {
            UnitType = unitType;
            UnitLevel = unitLevel;

            _infoText.text = $"{unitType}, {unitLevel}";
        }
        
    }
}