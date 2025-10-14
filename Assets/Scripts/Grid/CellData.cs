
namespace BusJamDemo.Grid
{
    public class CellData
    {
        public readonly int CellID; 
        public bool HasItem => HeldItem != null;
        public CellItem HeldItem { get; private set; }
        public CellPosition CellPosition { get; private set; }
        private static int _nextCellId = 0;
        public CellData(CellPosition cellPosition)
        {
            CellID = _nextCellId++;
            CellPosition = cellPosition;
        }
        
        public void FillItem(CellItem gridItem)
        {
            HeldItem = gridItem;
        }

        public void EraseItem()
        {
            HeldItem = null;
        }
    }
}