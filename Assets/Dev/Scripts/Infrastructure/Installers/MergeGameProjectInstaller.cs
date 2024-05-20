using Dev.Effects;
using Dev.UnitsLogic;
using UnityEngine;
using Zenject;

namespace Dev.Infrastructure.Installers
{
    public class MergeGameProjectInstaller : MonoInstaller
    {
        [SerializeField] private FxController _fxController;
        [SerializeField] private UnitsStaticDataContainer _unitsStaticDataContainer;

        public override void InstallBindings()
        {
            Container.Bind<FxController>().FromInstance(_fxController).AsSingle();
            Container.Bind<UnitsStaticDataContainer>().FromInstance(_unitsStaticDataContainer).AsSingle();
        }
    }
}