using BusJamDemo.Core;
using BusJamDemo.Service;
using UnityEngine;

namespace BusJamDemo.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private StartScreenUI startScreenPrefab;
        [SerializeField] private GameplayScreenUI gameplayScreenPrefab;
        [SerializeField] private EndGameScreenUI endGameScreenPrefab;

        private StartScreenUI _startScreen;
        private GameplayScreenUI _gameplayScreen;
        private EndGameScreenUI _endGameScreen;
        
        private IGameService _gameService;
        private ILevelService _levelService;
        private ITimerService _timerService;
        public void Initialize(IGameService gameService, ILevelService levelService, ITimerService timerService)
        {
            _gameService = gameService;
            _levelService = levelService;
            _timerService = timerService;
            _gameService.OnGameStateChanged += OnGameStateChanged;
        }

        public void ActivateSystem()
        {
            _startScreen = Instantiate(startScreenPrefab);
            _gameplayScreen = Instantiate(gameplayScreenPrefab);
            _endGameScreen = Instantiate(endGameScreenPrefab);

            _startScreen.Initialize(_gameService);
            _gameplayScreen.Initialize(_timerService, _levelService);
            _endGameScreen.Initialize(_gameService, _levelService);
            
            _startScreen.Hide();
            _gameplayScreen.Hide();
            _endGameScreen.Hide();
        }

        private void OnGameStateChanged(GameState newState)
        {
            _startScreen.Hide();
            _gameplayScreen.Hide();
            _endGameScreen.Hide();

            switch (newState)
            {
                case GameState.StartScreen:
                    _startScreen.Show();
                    break;
                case GameState.Gameplay:
                    _gameplayScreen.Show();
                    break;
                case GameState.LevelComplete:
                case GameState.LevelFail:
                    _endGameScreen.Show();
                    break;
            }
        }

        public void UpdateLevelDisplay(int level)
        {
            throw new System.NotImplementedException();
        }
    }
}