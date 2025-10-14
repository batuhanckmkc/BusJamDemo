using System.Collections.Generic;
using BusJamDemo.Grid;
using UnityEngine;

namespace BusJamDemo.Service
{
    public interface IGridService
    {
        CellData this[int row, int column] { get; }
        CellData GetEligibleBoardingCell();
        List<CellData> BoardingCells { get; }
        void GenerateBoardingCells(int boardingCellCount);
        void GenerateMainCells(int rows, int columns, float cellSize);
        void ClearGrid();
        bool AllBoardingCellsIsBusy();
        int RowCount { get; }
        int ColumnCount { get; }
        Transform Transform { get; }
    }
}