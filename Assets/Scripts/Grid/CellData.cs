using BusJamDemo.LevelLoad;

namespace BusJamDemo.Grid
{
    public class CellData
    {
        public bool HasItem => CellItem != null;
        public Cell Cell { get; private set; }
        public CellItem CellItem { get; private set; }
        public CellPosition CellPosition { get; private set; }
        public CellData_SO SourceSO { get; private set; }
        public CellData(CellPosition cellPosition)
        {
            CellPosition = cellPosition;
        }
        
        public void SetSourceSO(CellData_SO sourceSo)
        {
            SourceSO = sourceSo;
        }


        public void FillCell(Cell grid)
        {
            Cell = grid;
        }
        
        public void FillItem(CellItem gridItem)
        {
            CellItem = gridItem;
        }

        public void EraseItem()
        {
            CellItem = null;
        }
    }
}