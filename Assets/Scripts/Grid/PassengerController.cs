using System.Collections.Generic;
using BusJamDemo.Core;
using BusJamDemo.Service;
using BusJamDemo.Utility;
using UnityEngine;

namespace BusJamDemo.Grid
{
    public class PassengerController : MonoBehaviour, IPassengerService
    {
        private List<Passenger> _allPassengers = new();
        private readonly List<Passenger> _activePassengers = new();

        private IGameService _gameService;
        private IPoolService _poolService;
        public void Initialize(IGameService gameService, IPoolService poolService)
        {
            _gameService = gameService;
            _poolService = poolService;
            _gameService.OnGameStateChanged += StopPassengers;
            EventManager.Subscribe(GameplayEvents.LevelLoaded, RecalculateAllPassengerOutlines);
            EventManager<Passenger>.Subscribe(GameplayEvents.OnPassengerMove, OnPassengerMove);
        }

        private void OnDestroy()
        {
            EventManager.Unsubscribe(GameplayEvents.LevelLoaded, RecalculateAllPassengerOutlines);
            EventManager<Passenger>.Unsubscribe(GameplayEvents.OnPassengerMove, OnPassengerMove);
        }

        public void RegisterPassenger(Passenger passenger)
        {
            if (!_activePassengers.Contains(passenger))
            {
                _activePassengers.Add(passenger);
            }

            if (!_allPassengers.Contains(passenger))
            {
                _allPassengers.Add(passenger);
            }
        }

        public void DeregisterPassenger(Passenger passenger)
        {
            _activePassengers.Remove(passenger);
        }

        private void OnPassengerMove(Passenger passenger)
        {
            DeregisterPassenger(passenger);
            RecalculateAllPassengerOutlines();
        }
        
        private void RecalculateAllPassengerOutlines()
        {
            foreach (var passenger in _activePassengers)
            {
                passenger.CheckOutlineVisibility(); 
            }
        }

        private void StopPassengers(GameState gameState)
        {
            if (gameState == GameState.LevelFail)
            {
                foreach (var passenger in _activePassengers)
                {
                    passenger.Stop();
                }   
            }
        }
        
        public void ClearPassengers()
        {
            foreach (var passenger in _allPassengers)
            {
                _poolService.Release(passenger);
            }
            _allPassengers.Clear();
            _activePassengers.Clear();
        }
    }
}