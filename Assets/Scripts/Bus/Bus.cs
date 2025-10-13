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
        private readonly List<Passenger> _passengers = new ();
        private bool HasEmptySeat => _passengers.Count < _busContent.RequiredPassengerSequence.Count;
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
            _passengers.Add(passenger);
        }

        public void CheckBusState()
        {
            if (!HasEmptySeat && transform.childCount == _busContent.RequiredPassengerSequence.Count)
            {
                EventManager<Bus>.Execute(GameplayEvents.OnBusFull, this);
                transform.DOMove(new Vector3(15, transform.position.y, transform.position.z), 2f).OnComplete(() =>
                {
                    Destroy(gameObject);
                });
            }
        }
    }
}