namespace BusJamDemo.Grid
{
    public class CellData
    {
        public bool HasItem => CellItem != null;
        public Cell Grid { get; private set; }
        public CellItem CellItem { get; private set; }
        public CellPosition GridPosition { get; private set; }
        public CellData(CellPosition gridPosition)
        {
            GridPosition = gridPosition;
        }

        public void FillCell(Cell grid)
        {
            Grid = grid;
        }
        
        public void FillItem(CellItem gridItem)
        {
            CellItem = gridItem;
        }
    }
}