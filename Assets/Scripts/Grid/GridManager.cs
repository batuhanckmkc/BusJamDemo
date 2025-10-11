using System;
using BusJamDemo.Utility;
using UnityEngine;

namespace BusJamDemo.Grid
{
    public class GridManager : MonoBehaviour
    {
        [SerializeField] private Cell cellPrefab;
        private CellData[,] _gridCells;
        public CellData this[int row, int column] => _gridCells[row, column];
        //Z
        public int RowCount;
        //X
        public int ColumnCount;
        public float CellSize;

        private void Awake()
        {
            EventManager<ItemPlaceData>.Subscribe(BoardEvents.OnCellItemPlaced, OnCellItemPlaced);
            EventManager<ItemRemoveData>.Subscribe(BoardEvents.OnCellItemRemoved, OnCellItemRemoved);
        }

        private void OnDestroy()
        {
            EventManager<ItemPlaceData>.Unsubscribe(BoardEvents.OnCellItemPlaced, OnCellItemPlaced);
            EventManager<ItemRemoveData>.Unsubscribe(BoardEvents.OnCellItemRemoved, OnCellItemRemoved);
        }

        public void GenerateGrid(int rows, int columns, float cellSize)
        {
            RowCount = rows;
            ColumnCount = columns;
            CellSize = cellSize;
            
            _gridCells = new CellData[RowCount, ColumnCount];
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
                    
                    _gridCells[i, j] = cellData;
                }
            }
        }
        
        public void GenerateGrid()
        {
            _gridCells = new CellData[RowCount, ColumnCount];
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
                    
                    _gridCells[i, j] = cellData;
                }
            }
        }

        private void OnCellItemPlaced(ItemPlaceData placeData)
        {
            var itemPlacedCell = _gridCells[placeData.Coordinates.Row, placeData.Coordinates.Column];
            itemPlacedCell.FillItem(placeData.PlacedItem);
        }

        private void OnCellItemRemoved(ItemRemoveData removeData)
        {
            var itemRemovedCell = _gridCells[removeData.Coordinates.Row, removeData.Coordinates.Column];
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
        public readonly CellPosition Coordinates;
            
        public ItemRemoveData(CellPosition coordinates)
        {
            Coordinates = coordinates;
        }
    }
}