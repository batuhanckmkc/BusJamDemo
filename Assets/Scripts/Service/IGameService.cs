using System;
using BusJamDemo.Core;

namespace BusJamDemo.Service
{
    public interface IGameService
    {
        GameState CurrentState { get; }
        event Action<GameState> OnGameStateChanged;
        void UpdateGameState(GameState newState);
    }
}