using Dev.GridLogic;
using Dev.UI;
using Dev.UnitsLogic;
using UnityEngine;
using Zenject;

namespace Dev.Infrastructure.Installers
{
    public class MainInstaller : MonoInstaller
    {
        [SerializeField] private GridController _gridController;
        [SerializeField] private UnitsCardsService _unitsCardsService;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameState>().AsSingle().NonLazy();
            
            Container.Bind<GridController>().FromInstance(_gridController).AsSingle();
            Container.Bind<UnitsCardsService>().FromInstance(_unitsCardsService).AsSingle();
            Container.BindInterfacesAndSelfTo<InputService>().AsSingle().NonLazy();
        }

    }
}