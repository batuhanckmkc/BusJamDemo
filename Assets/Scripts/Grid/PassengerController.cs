using System.Collections.Generic;
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
            EventManager.Subscribe(GameplayEvents.LevelLoaded, RecalculateAllPassengerOutlines);
            EventManager.Subscribe(GameplayEvents.OnPassengerMove, RecalculateAllPassengerOutlines);
        }

        private void OnDisable()
        {
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
    }
}