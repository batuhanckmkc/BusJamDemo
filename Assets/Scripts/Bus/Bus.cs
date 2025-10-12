using System.Collections.Generic;
using BusJamDemo.Grid;
using BusJamDemo.LevelLoad;
using BusJamDemo.Utility;
using DG.Tweening;
using UnityEngine;

namespace BusJamDemo.Bus
{
    public class Bus : MonoBehaviour
    {
        [SerializeField] private MeshRenderer meshRenderer;
        public List<Passenger> Passengers = new ();
        public bool HasEmptySeat => Passengers.Count < _busContent.RequiredPassengerSequence.Count;
        private BusContent _busContent;
        public void Initialize(BusContent busContent)
        {
            _busContent = busContent;
            meshRenderer.material.color = _busContent.ColorType.GetColor();
        }

        public bool CanGetOn(Passenger passenger)
        {
            return HasEmptySeat && passenger.CellContent is PassengerContent passengerContent && _busContent.ColorType == passengerContent.ColorType;
        }
        
        public void GetOn(Passenger passenger)
        {
            Passengers.Add(passenger);
        }

        public void CheckBusState()
        {
            if (!HasEmptySeat && transform.childCount == _busContent.RequiredPassengerSequence.Count)
            {
                transform.DOMove(new Vector3(15, transform.position.y, transform.position.z), 2f);
                EventManager<Bus>.Execute(GameplayEvents.OnBusFull, this);   
            }
        }
    }
}