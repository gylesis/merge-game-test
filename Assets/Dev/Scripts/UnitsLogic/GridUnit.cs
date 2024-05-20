using Dev.Effects;
using UniRx;
using UnityEngine;

namespace Dev.UnitsLogic
{
    public abstract class GridUnit : MonoBehaviour
    {
        [SerializeField] private Transform _view;
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private Gradient _gradient;

        public abstract UnitType UnitType { get; }
        public Subject<Unit> OnDestroy { get; } = new Subject<Unit>();
        public int Id => gameObject.GetInstanceID();
        public Color CurrentColor => _gradient.colorKeys[_level % _gradient.colorKeys.Length - 1].color;

        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
        private int _level = 1;

        public Vector3 ScaleByLevel(int level)
        {
            return (Vector3.one * 0.25f) * level;
        }
        
        public virtual void OnLevelUp(int level, bool afterMerge = false)
        {
            _level = level;
            _view.localScale = ScaleByLevel(level);

            SetColor(CurrentColor);
        }

        public void SetPosition(Vector3 pos)
        {
            transform.position = pos;
           // _view.transform.position = pos + Vector3.up * (ScaleByLevel(_level).y / 2);
        }
        
        public void SetColor(Color color)
        {
            _meshRenderer.material.SetColor(BaseColor, color);
        }

        protected void SpawnFX(string effectName)
        {
            bool hasSpawnedEffect = FxController.Instance.TrySpawnEffectAt(effectName, transform.position + Vector3.up * 0.2f, out UnitUpgradeEffect effect);

            if (hasSpawnedEffect)
            {
                effect.SetParticlesColor( _meshRenderer.material.GetColor(BaseColor));
            }
        }
    }
}