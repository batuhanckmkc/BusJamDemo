using System.Collections.Generic;
using BusJamDemo.Core;
using BusJamDemo.Utility;
using UnityEngine;

namespace BusJamDemo.Grid
{
    public class PassengerController : MonoBehaviour
    {
        public static PassengerController Instance { get; private set; }
        private List<Passenger> _allPassengers = new();
        private readonly List<Passenger> _activePassengers = new();

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }
    
        private void OnEnable()
        {
            GameManager.OnGameStateChanged += StopPassengers;
            EventManager.Subscribe(GameplayEvents.LevelLoaded, RecalculateAllPassengerOutlines);
            EventManager<Passenger>.Subscribe(GameplayEvents.OnPassengerMove, OnPassengerMove);
        }

        private void OnDisable()
        {
            GameManager.OnGameStateChanged -= StopPassengers;
            EventManager.Unsubscribe(GameplayEvents.LevelLoaded, RecalculateAllPassengerOutlines);
            EventManager<Passenger>.Unsubscribe(GameplayEvents.OnPassengerMove, OnPassengerMove);
        }

        public void RegisterPassenger(Passenger passenger)
        {
            if (!_activePassengers.Contains(passenger))
            {
                _allPassengers.Add(passenger);
                _activePassengers.Add(passenger);
            }
        }

        private void DeregisterPassenger(Passenger passenger)
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
                Destroy(passenger.gameObject);
            }
            _allPassengers.Clear();
            _activePassengers.Clear();
        }
    }
}