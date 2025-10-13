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
            if (newState == GameState.LevelFail)
            {
                resultText.text = "Level Fail!";
                restartButton.gameObject.SetActive(true);
                nextLevelButton.gameObject.SetActive(false);
                gameObject.SetActive(true);
                TimerManager.Instance.StopTimer();
            }
            else if (newState == GameState.GameComplete)
            {
                resultText.text = "Congratulations! Game Completed!";
                restartButton.gameObject.SetActive(false);
                nextLevelButton.gameObject.SetActive(false);
                gameObject.SetActive(true);
                TimerManager.Instance.StopTimer();
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
