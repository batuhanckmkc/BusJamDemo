using UnityEngine;

namespace BusJamDemo.Grid
{
    public abstract class CellItem : MonoBehaviour
    {
        public CellData CellData;
        public abstract void Perform();
        public void Initialize(CellData cellData)
        {
            CellData = cellData;
        }
    }
}