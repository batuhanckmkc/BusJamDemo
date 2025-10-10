using UnityEngine;

namespace BusJamDemo.Grid
{
    public class Tunnel : CellItem
    {
        public override void Perform()
        {
            Debug.Log("Spawn Passenger");
        }
    }
}