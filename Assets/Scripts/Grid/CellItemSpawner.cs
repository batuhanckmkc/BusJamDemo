using UnityEngine;
using BusJamDemo.LevelLoad;
using BusJamDemo.Service;

namespace BusJamDemo.Grid
{
    public class CellItemSpawner : MonoBehaviour, ICellItemSpawner
    {
        private IGridService _gridService;
        private IPassengerService _passengerService;
        private IPathfindingService _pathfindingService;
        private IBusService _busService;
        private IGameService _gameService;
        private IPoolService _poolService;
        public void Initialize(IGridService gridService, IPassengerService passengerService, IPathfindingService pathfindingService, IBusService busService, IGameService gameService, IPoolService poolService)
        {
            _gridService = gridService;
            _passengerService = passengerService;
            _pathfindingService = pathfindingService;
            _busService = busService;
            _gameService = gameService;
            _poolService = poolService;
        }
        
        private CellItem GetAndSetupItem<T>(CellContent cellContent, int row, int col, Transform parent) where T : CellItem
        {
            if (row < 0 || row >= _gridService.RowCount || col < 0 || col >= _gridService.ColumnCount)
            {
                Debug.LogWarning($"[Spawner] Invalid coordinates: ({row}, {col})");
                return null;
            }

            var cellData = _gridService[row, col];
            if (cellData.HasItem)
            {
                Debug.LogWarning($"[Spawner] Grid cell already occupied: ({row}, {col})");
                return null;
            }
            
            var worldPos = cellData.CellPosition.WorldPosition;
            var cellItem = _poolService.Get<T>();
            cellItem.transform.SetPositionAndRotation(worldPos, Quaternion.identity);
            cellData.FillItem(cellItem);
            cellItem.Initialize(cellData, cellContent);
            return cellItem;
        }
        
        public Passenger SpawnPassenger(PassengerContent content, int row, int col, Transform parent)
        {
            var passenger = GetAndSetupItem<Passenger>(content, row, col, parent) as Passenger;
            
            if (passenger != null)
            {
                passenger.InitializeServices(_pathfindingService, _gridService, _busService, _gameService);
                passenger.SetColor();
                passenger.TrySetAnimation(PassengerAnimationState.Idle);
                _passengerService.RegisterPassenger(passenger);
            }
            return passenger;
        }

        public Tunnel SpawnTunnel(TunnelContent content, int row, int col, Transform parent)
        {
            var tunnel = GetAndSetupItem<Tunnel>(content, row, col, parent) as Tunnel;
            
            if (tunnel != null)
            {
                // TODO: Tunnel passenger sequence - tunnel.SetPassengerSequence(content.PassengerSequence)
            }
            return tunnel;
        }
    }
}