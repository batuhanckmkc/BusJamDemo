using System;
using BusJamDemo.Bus;
using UnityEngine;
using BusJamDemo.Grid;

namespace BusJamDemo.LevelLoad
{
    public class LevelLoader : MonoBehaviour
    {
        [Header("Required References")]
        [SerializeField] private GridManager gridManager;
        [SerializeField] private CellItemSpawner itemSpawner;
        [SerializeField] private BusController busController;

        [SerializeField] private float defaultCellSize = 1f;
        
        [Header("Load Level")]
        [SerializeField] private LevelData_SO currentLevelData;
        public LevelData_SO CurrentLevelData => currentLevelData;

        public static LevelLoader Instance;
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
            if (levelData.GridContents == null || levelData.GridContents.Count == 0)
            {
                Debug.LogError("[LevelLoader] LevelData's GridContents list is empty or null.");
                return;
            }

            gridManager.GenerateMainCells(levelData.Rows, levelData.Columns, defaultCellSize);
            gridManager.GenerateBoardingCells(levelData.BoardingCellContent.DefaultBoardingCellCount);
            busController.CreateBuses();

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
        }
        
        private void SpawnCellItem(CellContent content, int row, int col)
        {
            CellItem spawnedItem = null;
            
            switch (content.Type)
            {
                case CellContentType.Passenger:
                    var pContent = (PassengerContent)content;
                    spawnedItem = itemSpawner.SpawnPassenger(pContent, row, col, gridManager.transform);
                    break;
                case CellContentType.Tunnel:
                    var tContent = (TunnelContent)content;
                    spawnedItem = itemSpawner.SpawnTunnel(tContent, row, col, gridManager.transform);
                    break;
                case CellContentType.Empty:
                default:
                    break;
            }
    
            if (spawnedItem != null)
            {
                Debug.Log($"[LevelLoader] Item spawned: {content.Type} at ({row}, {col})");
            }
        }
    }
}