using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using BusJamDemo.Core;
using BusJamDemo.LevelLoad;
using BusJamDemo.Service;
using BusJamDemo.Utility;
using UnityEngine;

namespace BusJamDemo.Grid
{
    public class PassengerController : MonoBehaviour, IPassengerService
    {
        private List<int> _movedPassengers = new ();
        private List<Passenger> _allPassengers = new();
        private readonly List<Passenger> _activePassengers = new();

        private IGameService _gameService;
        private IPoolService _poolService;
        private ILevelService _levelService;
        public void Initialize(IGameService gameService, IPoolService poolService, ILevelService levelService)
        {
            _gameService = gameService;
            _poolService = poolService;
            _levelService = levelService;
            _movedPassengers = IntListFileSaver.LoadIntList();
            _gameService.OnGameStateChanged += StopPassengers;
            EventManager.Subscribe(GameplayEvents.LevelLoaded, OnLevelLoaded);
            EventManager<Passenger>.Subscribe(GameplayEvents.OnPassengerMove, OnPassengerMove);
        }

        private void OnDestroy()
        {
            EventManager.Unsubscribe(GameplayEvents.LevelLoaded, OnLevelLoaded);
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

        private void SimulateSave()
        {
            if (_movedPassengers.Count == _levelService.CurrentLevelData.GridContents.FindAll(cellContent => cellContent.Type == CellContentType.Passenger).Count)
            {
                IntListFileSaver.DeleteIntListFile();
                _levelService.AdvanceToNextLevel();
                _gameService.UpdateGameState(GameState.StartScreen);
                return;
            }
            
            if (IntListFileSaver.HasData())
            {
                foreach (var index in _movedPassengers)
                {
                    _allPassengers[index].InstantMove();
                }
                GameManager.ResumeGame = false; 
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
            if (!_movedPassengers.Contains(passenger.ID))
            {
                _movedPassengers.Add(passenger.ID);
                IntListFileSaver.SaveIntList(_movedPassengers);
            }
        }

        private void OnLevelLoaded()
        {
            RecalculateAllPassengerOutlines();
            SimulateSave();
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