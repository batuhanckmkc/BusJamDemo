using UnityEngine;

namespace BusJamDemo.Grid
{
    public class Passenger : CellItem
    {
        public override void Perform()
        {
            Debug.Log("Move");
        }
    }
}