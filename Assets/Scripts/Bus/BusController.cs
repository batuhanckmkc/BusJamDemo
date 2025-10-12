using System.Collections.Generic;
using BusJamDemo.Grid;
using BusJamDemo.LevelLoad;
using BusJamDemo.Utility;
using DG.Tweening;
using UnityEngine;

namespace BusJamDemo.Bus
{
    public class BusController : MonoBehaviour
    {
        [SerializeField] private Bus busPrefab;
        public Bus CurrentBus;
        public List<Bus> Buses = new();
        public static BusController Instance;
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
            EventManager<Bus>.Subscribe(GameplayEvents.OnBusFull, OnBusFull);
        }

        private void OnDestroy()
        {
            EventManager<Bus>.Unsubscribe(GameplayEvents.OnBusFull, OnBusFull);
        }

        private void OnBusFull(Bus bus)
        {
            Buses.Remove(bus);
            if (Buses.Count > 0)
            {
                CurrentBus = Buses[0];
                MoveBusesToStop(); 
            }
        }
        public void CreateBuses()
        {
            var levelData = LevelLoader.Instance.CurrentLevelData;
            var busContents = levelData.BusContents;
            int busCount = busContents.Count;

            float busWidth = busPrefab.transform.localScale.x;
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

                var bus = Instantiate(busPrefab, new Vector3(spawnX, 0, spawnZ), Quaternion.identity, transform);

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
            float spacing = LevelLoader.Instance.CurrentLevelData.BusSpacingX;
            float duration = 1f;
            float busWidth = busPrefab.transform.localScale.x;

            float targetZ = GridManager.Instance.BoardingCells[^1].CellPosition.WorldPosition.z + LevelLoader.Instance.CurrentLevelData.BusSpawnDistance.z;
            float totalItemWidth = busWidth + spacing;

            var movementSequence = DOTween.Sequence();
            for (int i = 0; i < Buses.Count; i++)
            {
                Bus bus = Buses[i];

                float targetX = -i * totalItemWidth;
                Vector3 targetPos = new Vector3(targetX, 0, targetZ);
                movementSequence.Join(bus.transform.DOMove(targetPos, duration).SetEase(Ease.OutCubic));
            }
            movementSequence.OnComplete(() =>
            {
                EventManager.Execute(GameplayEvents.OnBusArrivedToStop);
            });
        }
    }
}