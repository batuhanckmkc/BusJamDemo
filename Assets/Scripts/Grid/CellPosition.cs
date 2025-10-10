using UnityEngine;

namespace BusJamDemo.Grid
{
    public class CellPosition
    {
        public Vector3 WorldPosition;
        public int Row;
        public int Column;
        public CellPosition(Vector3 worldPosition, int row, int column)
        {
            WorldPosition = worldPosition;
            Row = row;
            Column = column;
        }

        public override string ToString()
        {
            return $"{Row + "," + Column + $"World Position: {WorldPosition}"}"; 
        }
    }
}