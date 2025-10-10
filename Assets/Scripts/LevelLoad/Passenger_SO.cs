using UnityEngine;

namespace BusJamDemo.LevelLoad
{
    [CreateAssetMenu(fileName = "Cell_Passenger", menuName = "BusJam/Grid Cells/Passenger")]
    public class Passenger_SO : CellData_SO
    {
        public ColorType Color = ColorType.Red;
        public override void LoadCell(LevelLoader manager, int row, int col)
        {
            // Manager.SpawnPassenger(row, col, Color);
            CellTypeName = "PASSENGER";
            Debug.Log($"[{CellTypeName}] loaded: ({row}, {col}) -> Color: {Color}");
        }
    }
}