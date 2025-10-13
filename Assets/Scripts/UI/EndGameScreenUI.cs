using BusJamDemo.Core;
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
    
        private void OnEnable()
        {
            restartButton.onClick.AddListener(OnRestartButtonClicked);
            nextLevelButton.onClick.AddListener(OnNextLevelButtonClicked);
            GameManager.OnGameStateChanged += OnGameStateChanged;
        }

        private void OnDisable()
        {
            restartButton.onClick.RemoveListener(OnRestartButtonClicked);
            nextLevelButton.onClick.RemoveListener(OnNextLevelButtonClicked);
            GameManager.OnGameStateChanged -= OnGameStateChanged;
        }

        private void OnGameStateChanged(GameState newState)
        {
            switch (newState)
            {
                case GameState.LevelFail:
                    resultText.text = "Level Fail!";
                    restartButton.gameObject.SetActive(true);
                    nextLevelButton.gameObject.SetActive(false);
                    Show();
                    TimerManager.Instance.StopTimer();
                    break;
                case GameState.LevelComplete:
                    resultText.text = "Level Success!";
                    restartButton.gameObject.SetActive(false);
                    nextLevelButton.gameObject.SetActive(true);
                    Show();
                    TimerManager.Instance.StopTimer();
                    break;
            }
        }

        private void OnRestartButtonClicked()
        {
            GameManager.Instance.UpdateGameState(GameState.Gameplay);
            Hide();
        }

        private void OnNextLevelButtonClicked()
        {
            LevelManager.Instance.AdvanceToNextLevel();
            Hide();
        }
    }
}
