using System.Linq;
using UnityEngine;
using BusJamDemo.LevelLoad;

namespace BusJamDemo.Grid
{
    public class CellItemSpawner : MonoBehaviour
    {
        [SerializeField] private GridManager gridGenerator;
        [SerializeField] private CellItem[] itemPrefabs; 
        private CellItem InstantiateAndSetupItem(CellItem prefab, CellContent cellContent, int row, int col, Transform parent)
        {
            if (row < 0 || row >= gridGenerator.RowCount || col < 0 || col >= gridGenerator.ColumnCount)
            {
                Debug.LogWarning($"[Spawner] Invalid coordinates: ({row}, {col})");
                return null;
            }

            var cellData = gridGenerator[row, col];
            if (cellData.HasItem)
            {
                Debug.LogWarning($"[Spawner] Grid cell already occupied: ({row}, {col})");
                return null;
            }
            
            var worldPos = cellData.CellPosition.WorldPosition;
            var cellItem = Instantiate(prefab, worldPos, Quaternion.identity);
            
            cellItem.transform.SetParent(parent, true); 
            cellData.FillItem(cellItem);
            cellItem.Initialize(cellData, cellContent);
            return cellItem;
        }
        
        public Passenger SpawnPassenger(PassengerContent content, int row, int col, Transform parent)
        {
            var passengerPrefab = GetPrefabByContent(content.Type);
            var passenger = InstantiateAndSetupItem(passengerPrefab, content, row, col, parent) as Passenger;
            
            if (passenger != null)
            {
                passenger.SetColor();
                passenger.SetAnimation(PassengerAnimationState.Idle);
                PassengerController.Instance.RegisterPassenger(passenger);
            }
            return passenger;
        }

        public Tunnel SpawnTunnel(TunnelContent content, int row, int col, Transform parent)
        {
            var tunnelPrefab = GetPrefabByContent(content.Type);
            var tunnel = InstantiateAndSetupItem(tunnelPrefab, content, row, col, parent) as Tunnel;
            
            if (tunnel != null)
            {
                // TODO: Tunnel passenger sequence - tunnel.SetPassengerSequence(content.PassengerSequence)
            }
            return tunnel;
        }
        
        private CellItem GetPrefabByContent(CellContentType contentType)
        {
            switch (contentType)
            {
                case CellContentType.Passenger:
                    return itemPrefabs.FirstOrDefault(p => p is Passenger);
                
                case CellContentType.Tunnel:
                    return itemPrefabs.FirstOrDefault(p => p is Tunnel); 
            }
            return null;
        }
    }
}