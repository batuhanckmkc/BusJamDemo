using System.Collections.Generic;
using BusJamDemo.Grid;
using BusJamDemo.LevelLoad;
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
        
        public void MoveBusesToStop()
        {
            float spacing = LevelLoader.Instance.CurrentLevelData.BusSpacingX;
            float duration = 1f;
            float busWidth = busPrefab.transform.localScale.x;

            float targetZ = GridManager.Instance.BoardingCells[^1].CellPosition.WorldPosition.z + LevelLoader.Instance.CurrentLevelData.BusSpawnDistance.z;
            float totalItemWidth = busWidth + spacing;

            for (int i = 0; i < Buses.Count; i++)
            {
                Bus bus = Buses[i];

                float targetX = -i * totalItemWidth;
                Vector3 targetPos = new Vector3(targetX, 0, targetZ);
                bus.transform.DOMove(targetPos, duration).SetEase(Ease.OutCubic);
            }
        }
    }
}