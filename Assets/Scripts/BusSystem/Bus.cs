using System.Collections.Generic;
using BusJamDemo.Core;
using BusJamDemo.Grid;
using BusJamDemo.LevelLoad;
using BusJamDemo.Service;
using BusJamDemo.Utility;
using DG.Tweening;
using UnityEngine;

namespace BusJamDemo.BusSystem
{
    public class Bus : MonoBehaviour
    {
        [SerializeField] private Transform busTransform;
        [SerializeField] private MeshRenderer meshRenderer;
        private readonly List<Passenger> _passengers = new ();
        private bool HasEmptySeat => _passengers.Count < _busContent.RequiredPassengerSequence.Count;
        public Transform BusTransform => busTransform;
        private BusContent _busContent;
        private IGameService _gameService;
        public void Initialize(BusContent busContent, IGameService gameService)
        {
            _busContent = busContent;
            _gameService = gameService;
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
            //TODO Refactor child count control
            if (!HasEmptySeat && transform.childCount == _busContent.RequiredPassengerSequence.Count + 1 && _gameService.CurrentState != GameState.LevelFail)
            {
                EventManager<Bus>.Execute(GameplayEvents.OnBusFull, this);
                transform.DOMove(new Vector3(15, transform.position.y, transform.position.z), 2f).OnComplete(() =>
                {
                    gameObject.SetActive(false);
                });
            }
        }
    }
}