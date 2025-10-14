using System;
using System.Collections.Generic;
using BusJamDemo.Core;
using BusJamDemo.LevelLoad;
using BusJamDemo.Service;
using BusJamDemo.Utility;
using DG.Tweening;
using UnityEngine;

namespace BusJamDemo.BusSystem
{
    public class BusController : MonoBehaviour, IBusService
    {
        [SerializeField] private Bus busParentPrefab;
        private readonly List<Bus> _allBuses = new();
        private readonly List<Bus> _buses = new();
        private Vector3 _stopPosition;
        public Bus CurrentBus { get; private set; }
        public Vector3 StopPosition => _stopPosition;

        private IGameService _gameService;
        private ILevelService _levelService;
        private IGridService _gridService;
        private IPoolService _poolService;
        public void Initialize(IGameService gameService, ILevelService levelService, IGridService gridService, IPoolService poolService)
        {
            _gameService = gameService;
            _levelService = levelService;
            _gridService = gridService;
            _poolService = poolService;
            
            EventManager<Bus>.Subscribe(GameplayEvents.OnBusFull, OnBusFull);
        }

        private void OnDestroy()
        {
            EventManager<Bus>.Unsubscribe(GameplayEvents.OnBusFull, OnBusFull);
        }

        public void CreateBuses(List<BusContent> busContents)
        {
            var levelData = _levelService.CurrentLevelData;
            int busCount = busContents.Count;

            float busWidth = busParentPrefab.BusTransform.localScale.x;
            float totalItemWidth = busWidth + levelData.BusSpacingX;

            float stopZPosition = _gridService.BoardingCells[^1].CellPosition.WorldPosition.z;
            float spawnZ = stopZPosition + levelData.BusSpawnDistance.z;
            float spawnXAnchor = -levelData.BusSpawnDistance.x;
            float totalSpanX = (busCount - 1) * totalItemWidth;
            for (int i = 0; i < busCount; i++)
            {
                float spawnX = spawnXAnchor - (i * totalItemWidth);
                float spawnXOffset = -totalSpanX;
                spawnX += spawnXOffset;

                var bus = _poolService.Get<Bus>();
                bus.transform.SetPositionAndRotation(new Vector3(spawnX, 0, spawnZ), Quaternion.identity);
                
                bus.Initialize(busContents[i], _gameService, _poolService);
                _buses.Add(bus);
                _allBuses.Add(bus);

                if (i == 0)
                {
                    CurrentBus = bus;
                }
            }

            if (GameManager.ResumeGame)
            {
                SkipBusMove();
            }
            else
            {
                MoveBusesToStop();
            }
        }

        public void ClearBuses()
        {
            foreach (var bus in _allBuses)
            {
                _poolService.Release(bus);
            }
            _allBuses.Clear();
            _buses.Clear();
        }
        
        private void OnBusFull(Bus bus)
        {
            _buses.Remove(bus);
            if (_buses.Count > 0)
            {
                CurrentBus = _buses[0];
                if (GameManager.ResumeGame)
                {
                    SkipBusMove();
                }
                else
                {
                    MoveBusesToStop();
                }
            }
            else
            {
                _gameService.UpdateGameState(GameState.LevelComplete);
            }
        }

        private void MoveBusesToStop()
        {
            var levelData = _levelService.CurrentLevelData;
            float spacing = levelData.BusSpacingX;
            float duration = 1f;
            float busWidth = busParentPrefab.BusTransform.localScale.x;

            float targetZ = _gridService.BoardingCells[^1].CellPosition.WorldPosition.z + levelData.BusSpawnDistance.z;
            float totalItemWidth = busWidth + spacing;

            var movementSequence = DOTween.Sequence();
            for (int i = 0; i < _buses.Count; i++)
            {
                Bus bus = _buses[i];

                float targetX = -i * totalItemWidth;
                Vector3 targetPos = new Vector3(targetX, 0, targetZ);
                if (i == 0)
                {
                    _stopPosition = targetPos;
                }
                movementSequence.Join(bus.transform.DOMove(targetPos, duration).SetEase(Ease.OutCubic));
            }
            movementSequence.OnComplete(() =>
            {
                EventManager.Execute(GameplayEvents.OnBusArrivedToStop);
            });
        }

        public void SkipBusMove()
        {
            var levelData = _levelService.CurrentLevelData;
            float spacing = levelData.BusSpacingX;
            float busWidth = busParentPrefab.BusTransform.localScale.x;

            float targetZ = _gridService.BoardingCells[^1].CellPosition.WorldPosition.z + levelData.BusSpawnDistance.z;
            float totalItemWidth = busWidth + spacing;
            for (int i = 0; i < _buses.Count; i++)
            {
                float targetX = -i * totalItemWidth;
                Vector3 targetPos = new Vector3(targetX, 0, targetZ);
                if (i == 0)
                {
                    _stopPosition = targetPos;
                }

                _buses[i].transform.position = targetPos;
                EventManager.Execute(GameplayEvents.OnBusArrivedToStop);
            }
        }
    }
}