using UnityEngine;

namespace BusJamDemo.LevelLoad
{
    public abstract class CellData_SO : ScriptableObject
    {
        [Tooltip("For quickly viewing the cell type in the editor.")]
        public string CellTypeName = "GENERIC_CELL";
        public abstract void LoadCell(LevelLoader manager, int row, int col);
    }
    
    public enum ColorType 
    { 
        None, 
        Red, 
        Blue, 
        Green, 
        Yellow 
    }
}