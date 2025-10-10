using System.Collections.Generic;
using UnityEngine;

namespace BusJamDemo.LevelLoad
{
    [CreateAssetMenu(fileName = "Cell_Tunnel", menuName = "BusJam/Grid Cells/Tunnel")]
    public class Tunnel_SO : CellData_SO
    {
        [Tooltip("The color sequence of passengers that will emerge from this tunnel.")]
        public List<Passenger_SO> PassengerSequence = new List<Passenger_SO>();
        public override void LoadCell(LevelLoader manager, int row, int col)
        {
            CellTypeName = "TUNNEL";
            // Manager.SpawnTunnel(row, col, this.PassengerSequence);
            Debug.Log($"[{CellTypeName}] loaded: ({row}, {col}) -> Exit Sequence Length: {PassengerSequence.Count}");
        }
    }
}