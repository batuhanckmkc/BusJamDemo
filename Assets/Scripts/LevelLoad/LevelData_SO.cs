using System;
using System.Collections.Generic;
using UnityEngine;

namespace BusJamDemo.LevelLoad
{
    [CreateAssetMenu(fileName = "NewLevel", menuName = "BusJam/Game/Level Data", order = 0)]
    public class LevelData_SO : ScriptableObject
    {
        [Header("Level Details")]
        public int LevelIndex;
        public float Timer = 60f;
        
        [Header("Grid Size")]
        public int Rows = 8;
        public int Columns = 8;

        [Header("Cell Arrangement")]
        public CellData_SO[] GridCells; 
    }
}