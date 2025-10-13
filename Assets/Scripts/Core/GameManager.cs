using System;
using BusJamDemo.UI;
using UnityEngine;

namespace BusJamDemo.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        private GameState _currentState;

        public static Action<GameState> OnGameStateChanged;
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            UpdateGameState(GameState.StartScreen);
        }

        public void UpdateGameState(GameState newState)
        {
            if (_currentState == newState) return;

            _currentState = newState;
            OnGameStateChanged?.Invoke(newState);
            switch (newState)
            {
                case GameState.StartScreen:
                    UIManager.Instance.StartScreenUI.Show();
                    UIManager.Instance.GameplayScreenUI.Hide();
                    break;
                case GameState.Gameplay:
                    TimerManager.Instance.StartTimer(LevelManager.Instance.CurrentLevelData.Time);
                    UIManager.Instance.GameplayScreenUI.Show();
                    UIManager.Instance.GameplayScreenUI.UpdateLevelDisplay(LevelManager.Instance.CurrentLevelNumber);
                    break;
                case GameState.LevelComplete:
                    UIManager.Instance.EndGameScreenUI.Show();
                    break;
                case GameState.LevelFail:
                    TimerManager.Instance.StopTimer();
                    UIManager.Instance.EndGameScreenUI.Show();
                    break;
            }
        }
    }
}