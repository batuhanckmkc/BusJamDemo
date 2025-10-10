using UnityEngine;

namespace BusJamDemo.LevelLoad
{
    [CreateAssetMenu(fileName = "Cell_Empty", menuName = "BusJam/Grid Cells/Empty")]
    public class EmptyCell_SO : CellData_SO
    {
        public override void LoadCell(LevelLoader manager, int row, int col)
        {
            CellTypeName = "EMPTY";
            // TODO: Empty cell loaded. LevelManager only creates the floor tile here.
            Debug.Log($"[{CellTypeName}] loaded: ({row}, {col})");
        }
    }
}