using System;
using UniRx;
using UnityEngine;

namespace Dev.Effects
{
    public class FxController : MonoBehaviour
    {
        [SerializeField] private FxContainer _fxContainer;

        public static FxController Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public bool TrySpawnEffectAt<TEffectType>(string effectName, Vector3 pos, out TEffectType effect, Quaternion rotation = default) where TEffectType : Effect
        {   
            effect = null;
            
            var hasEffect = _fxContainer.TryGetEffectDataByName(effectName, out var effectPrefab);

            if (hasEffect)
            {
                Quaternion rot = rotation;
                if (rot == default)
                {
                    rot = Quaternion.identity;
                }

                Effect instancedEffect = Instantiate(effectPrefab, pos, rot);

                if (instancedEffect.GetType() == typeof(TEffectType))
                {
                    effect = (TEffectType)instancedEffect;
                }
                
                Observable.Timer(TimeSpan.FromSeconds(4)).TakeUntilDestroy(this).Subscribe((l => { Destroy(instancedEffect.gameObject); }));
                return true;
            }

            return false;
        }   

    }
}