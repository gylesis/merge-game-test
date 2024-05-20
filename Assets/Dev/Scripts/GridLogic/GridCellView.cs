using DG.Tweening;
using UnityEngine;

namespace Dev.GridLogic
{
    public class GridCellView : MonoBehaviour
    {
        [SerializeField] private Transform placePos;
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private float _blinkTime = 0.4f;

        public Vector3 PlacePos => placePos.position;

        public Vector2Int CellDataIndex;
        private Color _originColor;
        private Sequence _sequence;
        private Tweener _colorTorRedTween;
        private Tweener _colorTorOriginTween;

        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
        
        private void Awake()
        {
            _originColor = _meshRenderer.material.GetColor(BaseColor);
        }

        public void SetBlinkingState(bool toBlink)
        {
            if (toBlink)
            {
                _sequence = DOTween.Sequence();

                _colorTorRedTween = DOVirtual.Color(_originColor, Color.red, _blinkTime / 2, value =>
                {
                    _meshRenderer.material.SetColor(BaseColor, value);
                });
                
                _colorTorOriginTween = DOVirtual.Color(Color.red, _originColor, _blinkTime / 2, value =>
                {   
                    _meshRenderer.material.SetColor(BaseColor, value);
                });
                
                _sequence
                    .Append(_colorTorRedTween)
                    .Append(_colorTorOriginTween)
                    .SetLoops(-1);

                _sequence.Play();
            }
            else
            {
                _sequence.Kill();
                
                _meshRenderer.material.SetColor(BaseColor, _originColor);
            }
        }
    }
}