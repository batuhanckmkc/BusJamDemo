using BusJamDemo.LevelLoad;
using UnityEngine;

namespace BusJamDemo.Grid
{
    public abstract class CellItem : MonoBehaviour
    {
        public CellData CellData;
        public CellContent CellContent;
        public abstract void Perform();
        public virtual void Initialize(CellData cellData, CellContent cellContent)
        {
            CellData = cellData;
            CellContent = cellContent;
        }
    }
}