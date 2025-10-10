using BusJamDemo.Core.Input;
using UnityEngine;

namespace BusJamDemo.Grid
{
    public class Passenger : CellItem, IClickable
    {
        public override void Perform()
        {
            Debug.Log("Move");
        }

        public void HandleClick()
        {
            Perform();
        }
    }
}