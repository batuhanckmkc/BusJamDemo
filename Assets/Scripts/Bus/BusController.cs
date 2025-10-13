using System.Collections.Generic;
using BusJamDemo.Core;
using BusJamDemo.Grid;
using BusJamDemo.Utility;
using DG.Tweening;
using UnityEngine;

namespace BusJamDemo.Bus
{
    public class BusController : MonoBehaviour
    {
        [SerializeField] private Bus busParentPrefab;
        [HideInInspector] public Bus CurrentBus;
        public List<Bus> Buses = new();
        public static BusController Instance;
        private Vector3 _stopPosition;
        public Vector3 StopPosition => _stopPosition;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void OnEnable()
        {
            EventManager<Bus>.Subscribe(GameplayEvents.OnBusFull, OnBusFull);
        }

        private void OnDisable()
        {
            EventManager<Bus>.Unsubscribe(GameplayEvents.OnBusFull, OnBusFull);
        }

        public void ClearBuses()
        {
            foreach (var bus in Buses)
            {
                Destroy(bus.gameObject);
            }
            Buses.Clear();
        }
        
        private void OnBusFull(Bus bus)
        {
            Buses.Remove(bus);
            if (Buses.Count > 0)
            {
                CurrentBus = Buses[0];
                MoveBusesToStop(); 
            }
            else
            {
                GameManager.Instance.UpdateGameState(GameState.LevelComplete);
            }
        }
        
        public void CreateBuses()
        {
            var levelData = LevelManager.Instance.CurrentLevelData;
            var busContents = levelData.BusContents;
            int busCount = busContents.Count;

            float busWidth = busParentPrefab.BusTransform.localScale.x;
            float totalItemWidth = busWidth + levelData.BusSpacingX;

            float stopZPosition = GridManager.Instance.BoardingCells[^1].CellPosition.WorldPosition.z;
            float spawnZ = stopZPosition + levelData.BusSpawnDistance.z;
            float spawnXAnchor = -levelData.BusSpawnDistance.x;
            float totalSpanX = (busCount - 1) * totalItemWidth;
            for (int i = 0; i < busCount; i++)
            {
                float spawnX = spawnXAnchor - (i * totalItemWidth);
                float spawnXOffset = -totalSpanX;
                spawnX += spawnXOffset;

                var bus = Instantiate(busParentPrefab, new Vector3(spawnX, 0, spawnZ), Quaternion.identity, transform);

                bus.Initialize(busContents[i]);
                Buses.Add(bus);

                if (i == 0)
                {
                    CurrentBus = bus;
                }
            }
            MoveBusesToStop();
        }

        private void MoveBusesToStop()
        {
            float spacing = LevelManager.Instance.CurrentLevelData.BusSpacingX;
            float duration = 1f;
            float busWidth = busParentPrefab.BusTransform.localScale.x;

            float targetZ = GridManager.Instance.BoardingCells[^1].CellPosition.WorldPosition.z + LevelManager.Instance.CurrentLevelData.BusSpawnDistance.z;
            float totalItemWidth = busWidth + spacing;

            var movementSequence = DOTween.Sequence();
            for (int i = 0; i < Buses.Count; i++)
            {
                Bus bus = Buses[i];

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
    }
}