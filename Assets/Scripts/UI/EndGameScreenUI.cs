using BusJamDemo.Core;
using BusJamDemo.Grid;
using BusJamDemo.Service;
using BusJamDemo.Utility;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace BusJamDemo.UI
{
    public class EndGameScreenUI : ScreenUI
    {
        [SerializeField] private TextMeshProUGUI resultText;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button nextLevelButton;
    
        private IGameService _gameService;
        private ILevelService _levelService;
        
        public void Initialize(IGameService gameService, ILevelService levelService)
        {
            _gameService = gameService;
            _levelService = levelService;
            _gameService.OnGameStateChanged += OnGameStateChanged;
        }
        
        private void OnEnable()
        {
            restartButton.onClick.AddListener(OnRestartButtonClicked);
            nextLevelButton.onClick.AddListener(OnNextLevelButtonClicked);
        }

        private void OnDisable()
        {
            restartButton.onClick.RemoveListener(OnRestartButtonClicked);
            nextLevelButton.onClick.RemoveListener(OnNextLevelButtonClicked);
        }

        private void OnGameStateChanged(GameState newState)
        {
            switch (newState)
            {
                case GameState.LevelFail:
                    IntListFileSaver.DeleteIntListFile();
                    resultText.text = "Level Fail!";
                    restartButton.gameObject.SetActive(true);
                    nextLevelButton.gameObject.SetActive(false);
                    Show();
                    break;
                case GameState.LevelComplete:
                    IntListFileSaver.DeleteIntListFile();
                    resultText.text = "Level Success!";
                    restartButton.gameObject.SetActive(false);
                    nextLevelButton.gameObject.SetActive(true);
                    Show();
                    break;
            }
        }

        private void OnRestartButtonClicked()
        {
            _gameService.UpdateGameState(GameState.StartScreen);
        }

        private void OnNextLevelButtonClicked()
        {
            _levelService.AdvanceToNextLevel();
            _gameService.UpdateGameState(GameState.StartScreen);
        }
    }
}
