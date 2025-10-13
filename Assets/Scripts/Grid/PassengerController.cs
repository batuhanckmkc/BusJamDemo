using System.Collections.Generic;
using BusJamDemo.Core;
using BusJamDemo.Utility;
using UnityEngine;

namespace BusJamDemo.Grid
{
    public class PassengerController : MonoBehaviour
    {
        public static PassengerController Instance { get; private set; }
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
            EventManager.Subscribe(GameplayEvents.OnPassengerMove, RecalculateAllPassengerOutlines);
        }

        private void OnDisable()
        {
            GameManager.OnGameStateChanged -= StopPassengers;
            EventManager.Unsubscribe(GameplayEvents.LevelLoaded, RecalculateAllPassengerOutlines);
            EventManager.Unsubscribe(GameplayEvents.OnPassengerMove, RecalculateAllPassengerOutlines);
        }

        public void RegisterPassenger(Passenger passenger)
        {
            if (!_activePassengers.Contains(passenger))
            {
                _activePassengers.Add(passenger);
            }
        }

        public void DeregisterPassenger(Passenger passenger)
        {
            _activePassengers.Remove(passenger);
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
                    passenger.CanClick = false;
                    passenger.SetAnimation(PassengerAnimationState.Idle);
                }   
            }
        }
    }
}