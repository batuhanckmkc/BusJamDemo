using System;
using UnityEngine;

namespace BusJamDemo.Core
{
    public class TimerManager : MonoBehaviour
    {
        public static TimerManager Instance { get; private set; }
        private float _currentTime;
        private bool _isTimerRunning;

        public float CurrentTime => _currentTime;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            if (!_isTimerRunning) return;

            _currentTime -= Time.deltaTime;
            if (_currentTime <= 0)
            {
                _isTimerRunning = false;
                GameManager.Instance.UpdateGameState(GameState.LevelFail);
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