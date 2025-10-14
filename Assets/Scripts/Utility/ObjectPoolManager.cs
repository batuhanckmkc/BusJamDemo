using System;
using System.Collections.Generic;
using BusJamDemo.BusSystem;
using BusJamDemo.Grid;
using BusJamDemo.LevelLoad;
using BusJamDemo.Service;
using UnityEngine;

namespace BusJamDemo.Utility
{
    public class ObjectPoolManager : MonoBehaviour, IPoolService
    {
        [Header("Pool Prefabs (Assigned in Inspector)")]
        [SerializeField] private Bus busPrefab;
        [SerializeField] private Passenger passengerPrefab;
        [SerializeField] private Tunnel tunnelPrefab;
        [SerializeField] private Cell cellPrefab;

        private Dictionary<Type, Queue<Component>> _pools;
        private Transform _poolRoot; // Parent for all inactive pooled objects

        private void Awake()
        {
            _pools = new Dictionary<Type, Queue<Component>>();
            
            // Create a root object for inactive pooled items to keep the hierarchy clean
            _poolRoot = new GameObject("[POOL_ROOT]").transform;
            DontDestroyOnLoad(_poolRoot.gameObject);
        }

        private Queue<Component> GetPool<T>() where T : Component
        {
            if (!_pools.TryGetValue(typeof(T), out var pool))
            {
                pool = new Queue<Component>();
                _pools[typeof(T)] = pool;
            }
            return pool;
        }

        public T Get<T>() where T : Component
        {
            var pool = GetPool<T>();
            if (pool.Count > 0)
            {
                var item = (T)pool.Dequeue();
                if (item is IPoolable poolableItem)
                {
                    poolableItem.OnGetFromPool();
                }
                item.gameObject.SetActive(true);
                return item;
            }
            
            return CreateNewItem<T>();
        }

        public void Release<T>(T item) where T : Component
        {
            if (item == null) return;
            
            if (item is IPoolable poolableItem)
            {
                poolableItem.OnReleaseToPool();
            }
            
            item.gameObject.SetActive(false);
            item.transform.SetParent(_poolRoot);
            var pool = GetPool<T>();
            if (!pool.Contains(item))
            {
                pool.Enqueue(item);
            }
        }

        public void ClearAllPools()
        {
            foreach (var pool in _pools.Values)
            {
                while (pool.Count > 0)
                {
                    Destroy(pool.Dequeue().gameObject);
                }
            }
            _pools.Clear();
        }
        
        public void PreloadLevelAssets(LevelData_SO levelData)
        {
            // 1. Calculate required counts
            var requiredCounts = CalculateRequiredCounts(levelData);

            // 2. Preload/Recycle items
            PreloadPool(requiredCounts[typeof(Bus)], busPrefab);
            PreloadPool(requiredCounts[typeof(Passenger)], passengerPrefab);
            PreloadPool(requiredCounts[typeof(Tunnel)], tunnelPrefab);
            PreloadCellPools(levelData);

            Debug.Log($"[PoolManager] Preloaded: Buses={requiredCounts[typeof(Bus)]}, Passengers={requiredCounts[typeof(Passenger)]}, Tunnels={requiredCounts[typeof(Tunnel)]}");
        }
        
        private void PreloadPool<T>(int requiredCount, T prefab) where T : Component
        {
            var pool = GetPool<T>();
            int itemsToCreate = requiredCount - pool.Count;

            // Create only the deficit items
            for (int i = 0; i < itemsToCreate; i++)
            {
                pool.Enqueue(CreateNewItem(prefab));
            }
        }

        private void PreloadCellPools(LevelData_SO levelData)
        {
            int totalMainCells = levelData.Rows * levelData.Columns;
            PreloadPool(totalMainCells, cellPrefab);
        }


        // Helper to calculate how many of each item are needed for the level
        private Dictionary<Type, int> CalculateRequiredCounts(LevelData_SO levelData)
        {
            var counts = new Dictionary<Type, int>
            {
                { typeof(Bus), levelData.BusContents.Count },
                { typeof(Passenger), 0 },
                { typeof(Tunnel), 0 },
                { typeof(Cell), 0 }
            };

            // Count Passengers and Tunnels required from the grid content
            foreach (var content in levelData.GridContents)
            {
                if (content.Type == CellContentType.Passenger)
                {
                    counts[typeof(Passenger)]++;
                }
                else if (content.Type == CellContentType.Tunnel)
                {
                    counts[typeof(Tunnel)]++;
                    // Also count passengers that will spawn from the tunnel
                    var tunnelContent = (TunnelContent)content;
                    counts[typeof(Passenger)] += tunnelContent.PassengerSequence.Count;
                }
            }
            return counts;
        }

        // Internal method to create a new item
        private T CreateNewItem<T>(T prefab = null) where T : Component
        {
            // Use the injected prefabs if none is provided
            if (prefab == null)
            {
                if (typeof(T) == typeof(Bus)) prefab = (T)(object)busPrefab;
                else if (typeof(T) == typeof(Passenger)) prefab = (T)(object)passengerPrefab;
                else if (typeof(T) == typeof(Tunnel)) prefab = (T)(object)tunnelPrefab;
                else if (typeof(T) == typeof(Cell)) prefab = (T)(object)cellPrefab;
                
                if (prefab == null)
                {
                    Debug.LogError($"Pool Error: Prefab for type {typeof(T).Name} is not assigned in the ObjectPoolManager or cannot be determined.");
                    return null;
                }
            }
            var item = Instantiate(prefab, _poolRoot);
            item.name = $"{typeof(T).Name}_Pooled_Clone";
            return item;
        }
    }
}