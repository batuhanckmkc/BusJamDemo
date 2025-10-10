using UnityEngine;

namespace BusJamDemo.Grid
{
    public class GridGenerator : MonoBehaviour
    {
        [SerializeField] private Cell cellPrefab;
        private CellData[,] _gridCells;
        public CellData this[int row, int column] => _gridCells[row, column];
        //Z
        public int RowCount;
        //X
        public int ColumnCount;
        public float CellSize;
    
        public void Awake()
        {
            GenerateGrid();
        }
    
        private void GenerateGrid()
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
            // PrintCells();
        }

        private void PrintCells()
        {
            for (int i = 0; i <= _gridCells.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= _gridCells.GetUpperBound(1); j++)
                {
                    Debug.Log(_gridCells[i,j].GridPosition);
                }
            }
        }
    }
}