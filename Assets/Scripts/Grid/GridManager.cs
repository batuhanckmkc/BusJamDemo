using System.Collections.Generic;
using System.Linq;
using BusJamDemo.Service;
using BusJamDemo.Utility;
using UnityEngine;

namespace BusJamDemo.Grid
{
    public class GridManager : MonoBehaviour, IGridService
    {
        private CellData[,] _mainCells;
        private readonly List<CellData> _boardingCells = new();
        public List<CellData> BoardingCells => _boardingCells;
        public CellData this[int row, int column] => _mainCells[row, column];
        private readonly List<Cell> _allCells = new();
        private readonly Dictionary<int, CellData> _allCellsById = new(); 
        public int RowCount { get; private set; }
        public int ColumnCount { get; private set; }
        public Transform Transform => transform;

        private float _cellSize;
        private IPoolService _poolService;
        public void Initialize(IPoolService poolService)
        {
            _poolService = poolService;
        }
        
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
            var cellsWidth = _cellSize * (boardingCellCount - 1);
            var topCellZ = _mainCells[RowCount - 1, 0].CellPosition.WorldPosition.z;
            var centerOffset = new Vector3(cellsWidth / 2, 0, 0);
            for (int i = 0; i < boardingCellCount; i++)
            {
                var worldPosition = new Vector3(i * _cellSize, 0, topCellZ + _cellSize * boardingCellsDistanceMultiplier) - centerOffset;
                var cellPosition = new CellPosition(worldPosition);
                var cellData = new CellData(cellPosition);
                
                CreateCell(worldPosition, cellData);
                _boardingCells.Add(cellData);
            }
        }
        
        public void GenerateMainCells(int rows, int columns, float cellSize)
        {
            RowCount = rows;
            ColumnCount = columns;
            _cellSize = cellSize;
            
            _mainCells = new CellData[RowCount, ColumnCount];
            var gridWidth = _cellSize * (ColumnCount - 1);
            var gridHeight = _cellSize * (RowCount - 1);
            var centerOffset = new Vector3(gridWidth / 2, 0, gridHeight / 2);
            
            for (int i = 0; i < RowCount; i++)
            {
                for (int j = 0; j < ColumnCount; j++)
                {
                    var worldPosition = new Vector3(j * _cellSize, 0, i * _cellSize) - centerOffset;
                    var cellPosition = new CellPosition(worldPosition, i, j);
                    var cellData = new CellData(cellPosition);

                    CreateCell(worldPosition, cellData);
                    _mainCells[i, j] = cellData;
                }
            }
        }

        private void CreateCell(Vector3 worldPosition, CellData cellData)
        {
            var cell = _poolService.Get<Cell>();
            cell.transform.SetPositionAndRotation(worldPosition, Quaternion.identity);
            cell.CellData = cellData;
            _allCells.Add(cell);
            _allCellsById.Add(cellData.CellID, cellData);
        }
        
        public void ClearGrid()
        {
            foreach (var cell in _allCells)
            {
                if (cell.CellData.HasItem)
                {
                    _poolService.Release(cell.CellData.HeldItem);
                }
                _poolService.Release(cell);
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
            if (_allCellsById.TryGetValue(removeData.TargetCellData.CellID, out var itemRemovedCell))
            {
                itemRemovedCell.EraseItem();
            }
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