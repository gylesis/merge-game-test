using Dev.Effects;
using UniRx;
using UnityEngine;

namespace Dev.UnitsLogic
{
    public abstract class GridUnit : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private Gradient _gradient;
        
        public abstract UnitType UnitType { get; }
        public Subject<Unit> OnDestroy { get; } = new Subject<Unit>();
        public int Id => gameObject.GetInstanceID();
        public Color CurrentColor => _gradient.colorKeys[_level % _gradient.colorKeys.Length - 1].color;

        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
        private int _level = 1;

        public virtual void OnLevelUp(int level, bool afterMerge = false)
        {
            _level = level;
            transform.localScale = (Vector3.one * 0.25f) * level;

            SetColor(CurrentColor);
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