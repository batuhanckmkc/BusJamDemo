using BusJamDemo.Core.Input;
using UnityEngine;

namespace BusJamDemo.Grid
{
    public class Tunnel : CellItem, IClickable, IBlocker
    {
        public override void Perform()
        {
            Debug.Log("Spawn Passenger");
        }

        public void HandleClick()
        {
            Perform();
        }
    }
}