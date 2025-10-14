using UnityEngine;
using BusJamDemo.Service;
using BusJamDemo.Utility;

namespace BusJamDemo.LevelLoad
{
    public class LevelLoader : MonoBehaviour, ILevelLoader
    {
        private IGridService _gridService;
        private IBusService _busService;
        private IPassengerService _passengerService;
        private ICellItemSpawner _cellItemSpawner;
        public void Initialize(IGridService gridService, IBusService busService, IPassengerService passengerService, ICellItemSpawner cellItemSpawner)
        {
            _gridService = gridService;
            _busService = busService;
            _passengerService = passengerService;
            _cellItemSpawner = cellItemSpawner;
        }
        
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

            _gridService.GenerateMainCells(levelData.Rows, levelData.Columns, levelData.CellSize);
            _gridService.GenerateBoardingCells(levelData.BoardingCellContent.DefaultBoardingCellCount);
            _busService.CreateBuses(levelData.BusContents);
            
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
        
        public void ClearPreviousLevel()
        {
            _gridService.ClearGrid(); 
            _busService.ClearBuses();
            _passengerService.ClearPassengers();
        }

        private void SpawnCellItem(CellContent content, int row, int col)
        {
            switch (content.Type)
            {
                case CellContentType.Passenger:
                    var pContent = (PassengerContent)content;
                    _cellItemSpawner.SpawnPassenger(pContent, row, col, _gridService.Transform);
                    break;
                case CellContentType.Tunnel:
                    var tContent = (TunnelContent)content;
                    _cellItemSpawner.SpawnTunnel(tContent, row, col, _gridService.Transform);
                    break;
                case CellContentType.Empty:
                default:
                    break;
            }
        }
    }
}