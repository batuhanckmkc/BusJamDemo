using UnityEngine;
using BusJamDemo.Grid;

namespace BusJamDemo.LevelLoad
{
    public class LevelLoader : MonoBehaviour
    {
        [Header("Required References")]
        [SerializeField] private GridManager gridManager;
        [SerializeField] private CellItemSpawner itemSpawner;
        [SerializeField] private float defaultCellSize = 1f;
        
        [Header("Load Level")]
        [SerializeField] private LevelData_SO currentLevelData;

        private void Start()
        {
            if (currentLevelData != null)
            {
                LoadLevel(currentLevelData);
            }
            else
            {
                Debug.LogError("[LevelLoader] No LevelData_SO assigned to load!");
            }
        }

        private void LoadLevel(LevelData_SO levelData)
        {
            if (levelData.GridCells == null)
            {
                Debug.LogError("[LevelLoader] LevelData's GridCells array is empty or null.");
                return;
            }

            gridManager.GenerateGrid(levelData.Rows, levelData.Columns, defaultCellSize);
            
            int requiredLength = levelData.Rows * levelData.Columns;

            for (int i = 0; i < levelData.Rows; i++)
            {
                for (int j = 0; j < levelData.Columns; j++)
                {
                    int index = i * levelData.Columns + j;
                    
                    if (index >= requiredLength) 
                    {
                        Debug.LogError("[LevelLoader] Array overflow! Level data is corrupted.");
                        break;
                    }

                    CellData_SO cellData = levelData.GridCells[index];
                    if (cellData != null)
                    {
                        cellData.LoadCell(null, i, j);
                        if (!(cellData is EmptyCell_SO))
                        {
                            CellItem spawnedItem = itemSpawner.SpawnItem(cellData, i, j, gridManager.transform);
                            if (spawnedItem != null)
                            {
                                Debug.Log($"[LevelLoader] Item spawned: {cellData.CellTypeName} at ({i}, {j})");
                            }
                        }
                    }
                }
            }
            Debug.Log($"Level '{levelData.name}' loaded successfully.");
        }
    }
}