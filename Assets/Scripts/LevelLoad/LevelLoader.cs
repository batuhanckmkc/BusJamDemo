using BusJamDemo.Bus;
using UnityEngine;
using BusJamDemo.Grid;
using BusJamDemo.Utility;

namespace BusJamDemo.LevelLoad
{
    public class LevelLoader : MonoBehaviour
    {
        [Header("Required References")]
        [SerializeField] private CellItemSpawner itemSpawner;
        
        public void LoadLevel(LevelData_SO levelData)
        {
            if (levelData == null)
            {
                Debug.LogError("[LevelLoader] LevelData_SO is null. Cannot load level.");
                return;
            }
            
            ClearPreviousLevel();

            if (levelData.GridContents == null || levelData.GridContents.Count == 0)
            {
                Debug.LogError("[LevelLoader] LevelData's GridContents list is empty or null.");
                return;
            }

            GridManager.Instance.GenerateMainCells(levelData.Rows, levelData.Columns, levelData.CellSize);
            GridManager.Instance.GenerateBoardingCells(levelData.BoardingCellContent.DefaultBoardingCellCount);
            BusController.Instance.CreateBuses();
            
            int requiredLength = levelData.Rows * levelData.Columns;

            for (int i = 0; i < levelData.Rows; i++)
            {
                for (int j = 0; j < levelData.Columns; j++)
                {
                    int index = i * levelData.Columns + j;
                    
                    if (index >= requiredLength || index >= levelData.GridContents.Count) 
                    {
                        Debug.LogError("[LevelLoader] Level data is corrupted: Index overflow.");
                        break;
                    }

                    CellContent cellContent = levelData.GridContents[index];
                    SpawnCellItem(cellContent, i, j);
                }
            }
            
            Debug.Log($"Level '{levelData.name}' loaded successfully.");
            EventManager.Execute(GameplayEvents.LevelLoaded);
        }
        
        private void ClearPreviousLevel()
        {
            GridManager.Instance.ClearGrid(); 
            BusController.Instance.ClearBuses();
            PassengerController.Instance.ClearPassengers();
        }

        private void SpawnCellItem(CellContent content, int row, int col)
        {
            switch (content.Type)
            {
                case CellContentType.Passenger:
                    var pContent = (PassengerContent)content;
                    itemSpawner.SpawnPassenger(pContent, row, col, GridManager.Instance.transform);
                    break;
                case CellContentType.Tunnel:
                    var tContent = (TunnelContent)content;
                    itemSpawner.SpawnTunnel(tContent, row, col, GridManager.Instance.transform);
                    break;
                case CellContentType.Empty:
                default:
                    break;
            }
        }
    }
}