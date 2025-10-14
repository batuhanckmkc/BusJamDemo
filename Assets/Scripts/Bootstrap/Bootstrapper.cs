using BusJamDemo.BusSystem;
using BusJamDemo.Core;
using BusJamDemo.Grid;
using BusJamDemo.LevelLoad;
using BusJamDemo.Service;
using BusJamDemo.UI;
using BusJamDemo.Utility;
using UnityEngine;

namespace BusJamDemo.Bootstrap
{
    public class Bootstrapper
    {
        public static void Execute() 
        {
            var systemsPrefab = Resources.Load<GameObject>("Systems");
            if (systemsPrefab == null)
            {
                Debug.LogError("FATAL ERROR: 'Systems' Prefab not found in Resources. DI cannot be completed.");
                return;
            }
            
            var systemsRoot = Object.Instantiate(systemsPrefab);
            Object.DontDestroyOnLoad(systemsRoot);

            var poolManager = systemsRoot.GetComponent<ObjectPoolManager>();
            var gameManager = systemsRoot.GetComponent<GameManager>();
            var timerManager = systemsRoot.GetComponent<TimerManager>();
            var levelManager = systemsRoot.GetComponent<LevelManager>();
            var uiManager = systemsRoot.GetComponent<UIManager>();
            var gridManager = systemsRoot.GetComponent<GridManager>();
            var busController = systemsRoot.GetComponent<BusController>();
            var passengerController = systemsRoot.GetComponent<PassengerController>();
            var itemSpawner = systemsRoot.GetComponent<CellItemSpawner>();
            var levelLoader = systemsRoot.GetComponent<LevelLoader>();

            var pathfinder = new Pathfinder();

            IGameService gameService = gameManager;
            ITimerService timerService = timerManager;
            ILevelService levelService = levelManager;
            IGridService gridService = gridManager;
            IPathfindingService pathfindingService = pathfinder;
            IBusService busService = busController;
            IPassengerService passengerService = passengerController;
            ILevelLoader levelLoaderService = levelLoader;
            ICellItemSpawner itemSpawnerService = itemSpawner;
            IPoolService poolService = poolManager;
            
            gameManager.Initialize();
            gridManager.Initialize(poolService);
            pathfinder.Initialize(gridService);
            itemSpawner.Initialize(gridService, passengerService, pathfindingService, busService, gameService, poolService);
            levelManager.Initialize(gameService, levelLoaderService);
            levelLoader.Initialize(gridService, busService, passengerService, itemSpawnerService, poolService);
            
            passengerController.Initialize(gameService, poolService);
            busController.Initialize(gameService, levelService, gridService, poolService);
            timerManager.Initialize(gameService, levelService);
            uiManager.Initialize(gameService, levelService, timerService);
            
            levelManager.ActivateSystem();
            uiManager.ActivateSystem();
            
            EventManager.Execute(GameplayEvents.SystemInitialized);
            Debug.Log("BusJamDemo Systems Initialized Successfully (Composition Root Complete)");
        }
    }
}