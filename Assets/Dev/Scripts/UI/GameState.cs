using Dev.GridLogic;
using Zenject;

namespace Dev.UI
{
    public class GameState : ITickable
    {
        private GridController _gridController;

        public int Score { get; private set; }

        private const int ScorePerLevel = 50;
        
        public GameState(GridController gridController)
        {
            _gridController = gridController;
        }

        public void Tick()
        {
            Score = _gridController.BusyCellsCount * (ScorePerLevel * _gridController.MaxUnitLevel);
        }
    }
}