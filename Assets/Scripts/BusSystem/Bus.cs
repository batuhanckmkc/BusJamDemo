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
    public class Bus : MonoBehaviour, IPoolable
    {
        [SerializeField] private Transform busTransform;
        [SerializeField] private MeshRenderer meshRenderer;
        public Vector3 TargetSeat => _seatTransforms[_passengers.Count - 1];
        private readonly List<Vector3> _seatTransforms = new();
        private readonly List<Passenger> _passengers = new ();
        private bool HasEmptySeat => _passengers.Count < _busContent.RequiredPassengerSequence.Count;
        private float _seatSpacing;
        public Transform BusTransform => busTransform;
        private BusContent _busContent;
        private IGameService _gameService;
        private IPoolService _poolService;
        public void Initialize(BusContent busContent, IGameService gameService, IPoolService poolService)
        {
            _busContent = busContent;
            _gameService = gameService;
            _poolService = poolService;
            meshRenderer.material.color = _busContent.ColorType.GetColor();
            CalculateTargetSeatPosition();
        }

        private void CalculateTargetSeatPosition()
        {
            var busScaleX = busTransform.localScale.x;
            var passengerCount = _busContent.RequiredPassengerSequence.Count;
    
            _seatTransforms.Clear();
            if (passengerCount == 0) return;

            var slotWidth = busScaleX / passengerCount; 
            var leftmostX = -(busScaleX / 2f); 
            var initialOffset = leftmostX + (slotWidth / 2f); 
            for (int i = 0; i < passengerCount; i++)
            {
                float seatX = initialOffset + (i * slotWidth);
        
                _seatTransforms.Add(new Vector3(seatX, 0, 0)); 
            }
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
            if (!HasEmptySeat && transform.childCount == _busContent.RequiredPassengerSequence.Count + 1 && _gameService.CurrentState != GameState.LevelFail)
            {
                EventManager<Bus>.Execute(GameplayEvents.OnBusFull, this);
                var targetPos = new Vector3(15, transform.position.y, transform.position.z);
                if (GameManager.ResumeGame)
                {
                    transform.position = targetPos;
                    _poolService.Release(this);
                }
                else
                {
                    transform.DOMove(new Vector3(15, transform.position.y, transform.position.z), 2f).OnComplete(() =>
                    {
                        _poolService.Release(this);
                    });
                }
            }
        }

        public void OnReleaseToPool()
        {
            transform.DOKill(true);
            _passengers.Clear();
        }

        public void OnGetFromPool() { }
    }
}