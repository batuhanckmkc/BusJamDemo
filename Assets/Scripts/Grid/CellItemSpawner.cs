using UnityEngine;
using System.Linq;
using BusJamDemo.LevelLoad;

namespace BusJamDemo.Grid
{
    public class CellItemSpawner : MonoBehaviour
    {
        [SerializeField] private GridGenerator gridGenerator;
        [SerializeField] private CellItem[] itemPrefabs; 

        public CellItem SpawnItem(CellData_SO data, int row, int column, Transform parent)
        {
            if (row < 0 || row >= gridGenerator.RowCount || column < 0 || column >= gridGenerator.ColumnCount)
            {
                Debug.LogWarning($"[Spawner] Invalid coordinates: ({row}, {column})");
                return null;
            }

            var cellData = gridGenerator[row, column];
            if (cellData.HasItem)
            {
                Debug.LogWarning($"[Spawner] Grid cell already occupied: ({row}, {column})");
                return null;
            }
            
            CellItem prefabToSpawn = GetPrefabForType(data);

            if (prefabToSpawn == null)
            {
                Debug.LogError($"[Spawner] Prefab not found for {data.CellTypeName}.");
                return null;
            }

            var worldPos = cellData.GridPosition.WorldPosition;
            var cellItem = Instantiate(prefabToSpawn, worldPos, Quaternion.identity);
            cellItem.transform.SetParent(parent, true); 
            cellData.FillItem(cellItem);
            return cellItem;
        }

        private CellItem GetPrefabForType(CellData_SO data)
        {
            if (data is Passenger_SO)
            {
                return itemPrefabs.FirstOrDefault(p => p is Passenger); 
            }
            if (data is Tunnel_SO)
            {
                 return itemPrefabs.FirstOrDefault(p => p is Tunnel); 
            }
            
            if (data is EmptyCell_SO)
            {
                return null; 
            }

            return null;
        }
    }
}