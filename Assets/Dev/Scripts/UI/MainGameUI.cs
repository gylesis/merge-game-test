using TMPro;
using UnityEngine;
using Zenject;

namespace Dev.UI
{
    public class MainGameUI : PopUp
    {
        [SerializeField] private TMP_Text _scoreText;

        private GameState _gameState;

        protected override void Awake() { }

        [Inject]
        private void Construct(GameState gameState)
        {
            _gameState = gameState;
        }

        private void Update()
        {
            if (Time.frameCount % 15 == 0)
            {
                _scoreText.text = $"Score: {_gameState.Score}";
            }
        }
    }
}