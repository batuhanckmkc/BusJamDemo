using System;
using BusJamDemo.Service;
using UnityEngine;

namespace BusJamDemo.Core
{
    public class TimerManager : MonoBehaviour, ITimerService
    {
        private IGameService _gameService;
        private ILevelService _levelService;
        private float _currentTime;
        private bool _isTimerRunning;
        public float CurrentTime => _currentTime;
        public Action<float> OnTimerUpdate { get; set; }
        public void Initialize(IGameService gameService, ILevelService levelService)
        {
            _gameService = gameService;
            _levelService = levelService;
            _gameService.OnGameStateChanged += OnGameStateChanged;
        }

        private void Update()
        {
            if (!_isTimerRunning) return;

            _currentTime -= Time.deltaTime;
            OnTimerUpdate?.Invoke(_currentTime);
            if (_currentTime <= 0)
            {
                _isTimerRunning = false;
                _gameService.UpdateGameState(GameState.LevelFail);
            }
        }

        private void OnGameStateChanged(GameState newState)
        {
            switch (newState)
            {
                case GameState.Gameplay:
                    StartTimer(_levelService.CurrentLevelData.Time);
                    break;
                case GameState.LevelFail:
                    StopTimer();
                    break;
            }
        }

        public void StartTimer(float duration)
        {
            _currentTime = duration;
            _isTimerRunning = true;
        }

        public void StopTimer()
        {
            _isTimerRunning = false;
        }
    }
}