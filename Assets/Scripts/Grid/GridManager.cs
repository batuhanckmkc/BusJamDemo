using System.Collections.Generic;
using System.Linq;
using BusJamDemo.Service;
using BusJamDemo.Utility;
using UnityEngine;

namespace BusJamDemo.Grid
{
    public class GridManager : MonoBehaviour, IGridService
    {
        [SerializeField] private Cell cellPrefab;
        private CellData[,] _mainCells;
        private readonly List<CellData> _boardingCells = new();
        public List<CellData> BoardingCells => _boardingCells;
        public CellData this[int row, int column] => _mainCells[row, column];
        private readonly List<Cell> _allCells = new();
        private readonly Dictionary<int, CellData> _allCellsById = new(); 
        public int RowCount { get; private set; }
        public int ColumnCount { get; private set; }
        public Transform Transform => transform;

        public float CellSize;

        private void OnEnable()
        {
            EventManager<ItemPlaceData>.Subscribe(GameplayEvents.OnCellItemPlaced, OnCellItemPlaced);
            EventManager<ItemRemoveData>.Subscribe(GameplayEvents.OnCellItemRemoved, OnCellItemRemoved);
        }

        private void OnDisable()
        {
            EventManager<ItemPlaceData>.Unsubscribe(GameplayEvents.OnCellItemPlaced, OnCellItemPlaced);
            EventManager<ItemRemoveData>.Unsubscribe(GameplayEvents.OnCellItemRemoved, OnCellItemRemoved);
        }

        public void GenerateBoardingCells(int boardingCellCount)
        {
            var boardingCellsDistanceMultiplier = 3;
            var cellsWidth = CellSize * (boardingCellCount - 1);
            var topCellZ = _mainCells[RowCount - 1, 0].CellPosition.WorldPosition.z;
            var centerOffset = new Vector3(cellsWidth / 2, 0, 0);
            for (int i = 0; i < boardingCellCount; i++)
            {
                var worldPosition = new Vector3(i * CellSize, 0, topCellZ + CellSize * boardingCellsDistanceMultiplier) - centerOffset;
                var cellPosition = new CellPosition(worldPosition);
                var cellData = new CellData(cellPosition);
                
                var boardingCell = Instantiate(cellPrefab, worldPosition, Quaternion.identity, transform);
                cellData.FillCell(boardingCell);
                _allCells.Add(boardingCell);
                _allCellsById.Add(cellData.CellID, cellData); 
                _boardingCells.Add(cellData);
            }
        }
        
        public void GenerateMainCells(int rows, int columns, float cellSize)
        {
            RowCount = rows;
            ColumnCount = columns;
            CellSize = cellSize;
            
            _mainCells = new CellData[RowCount, ColumnCount];
            var gridWidth = CellSize * (ColumnCount - 1);
            var gridHeight = CellSize * (RowCount - 1);
            var centerOffset = new Vector3(gridWidth / 2, 0, gridHeight / 2);
            
            for (int i = 0; i < RowCount; i++)
            {
                for (int j = 0; j < ColumnCount; j++)
                {
                    var worldPosition = new Vector3(j * CellSize, 0, i * CellSize) - centerOffset;
                    var cellPosition = new CellPosition(worldPosition, i, j);
                    var cellData = new CellData(cellPosition);
                    
                    var cell = Instantiate(cellPrefab, worldPosition, Quaternion.identity, transform);
                    cellData.FillCell(cell);
                    _allCells.Add(cell);
                    _allCellsById.Add(cellData.CellID, cellData); 
                    _mainCells[i, j] = cellData;
                }
            }
        }

        public void ClearGrid()
        {
            foreach (var cell in _allCells)
            {
                Destroy(cell.gameObject);
            }
            _allCells.Clear();
            _boardingCells.Clear();
            _allCellsById.Clear();
            _mainCells = null;
        }
        
        public bool AllBoardingCellsIsBusy()
        {
            return _boardingCells.All(cellData => cellData.HasItem);
        }
        
        public CellData GetEligibleBoardingCell()
        {
            return _boardingCells.FirstOrDefault(cellData => !cellData.HasItem);
        }
        
        private void OnCellItemPlaced(ItemPlaceData placeData)
        {
            var itemPlacedCell = _mainCells[placeData.Coordinates.Row, placeData.Coordinates.Column];
            itemPlacedCell.FillItem(placeData.PlacedItem);
        }

        private void OnCellItemRemoved(ItemRemoveData removeData)
        {
            var itemRemovedCell = _allCellsById[removeData.TargetCellData.CellID];
            itemRemovedCell.EraseItem();
        }
    }
    
    public struct ItemPlaceData
    {
        public readonly CellItem PlacedItem;
        public readonly CellPosition Coordinates;

        public ItemPlaceData(CellItem placedItem, CellPosition coordinates)
        {
            PlacedItem = placedItem;
            Coordinates = coordinates;
        }
    }
        
    public struct ItemRemoveData
    {
        public readonly CellData TargetCellData; 
        public ItemRemoveData(CellData targetCellData)
        {
            TargetCellData = targetCellData;
        }
    }
}