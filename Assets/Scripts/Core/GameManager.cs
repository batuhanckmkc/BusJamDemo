using System;
using BusJamDemo.Service;
using BusJamDemo.Utility;
using UnityEngine;

namespace BusJamDemo.Core
{
    public class GameManager : MonoBehaviour, IGameService
    {
        public GameState CurrentState { get; private set; } = GameState.None;
        public event Action<GameState> OnGameStateChanged;

        public static bool ResumeGame;
        public void Initialize()
        {
            ResumeGame = IntListFileSaver.HasData();
            EventManager.Subscribe(GameplayEvents.SystemInitialized, StartGame);
        }
        private void StartGame() => UpdateGameState(GameState.StartScreen);
        
        public void UpdateGameState(GameState newState)
        {
            if (CurrentState == newState) return;

            CurrentState = newState;
            OnGameStateChanged?.Invoke(newState);
        }
    }
}