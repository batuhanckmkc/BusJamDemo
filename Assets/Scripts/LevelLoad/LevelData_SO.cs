using System;
using System.Collections.Generic;
using UnityEngine;

namespace BusJamDemo.LevelLoad
{
    [CreateAssetMenu(fileName = "Level_", menuName = "BusJamGame/Level Data", order = 0)]
    public class LevelData_SO : ScriptableObject
    {
        [Header("Level Details")]
        public int LevelIndex;
        public int Time;
        public int Rows = 8;
        public int Columns = 8;
        [SerializeReference]
        public BoardingCellContent BoardingCellContent;
        [SerializeReference]
        public List<BusContent> BusContents = new (); 
        [Header("--- GAME MECHANICS DATA ---")]

        [Tooltip("Stores the type and data of each cell in the grid.")]
        [SerializeReference]
        public List<CellContent> GridContents; 
    }
    
        public enum CellContentType { Empty = -1, Passenger = 5, Tunnel = 10 }
        public enum ColorType { Red = 0, Blue = 1, Green = 2, Yellow = 3 }
        public enum PassengerType { Standard = 0, Vip = 5 }
    
        [Serializable]
        public class BusDefinitionData : BusContent
        {
            public List<PassengerContent> RequiredPassengerSequence = new List<PassengerContent>();
        }
        
        [Serializable]
        public abstract class BusContent
        {
            public ColorType Color;
        }
        
        [Serializable]
        public class BoardingCellContent
        {
            public int DefaultBoardingCellCount = 5;
        }

        // Base class for all grid cell data
        [Serializable]
        public abstract class CellContent
        {
            public CellContentType Type;
            public abstract string GetTypeName(); 
        }

        [Serializable]
        public class PassengerContent : CellContent
        {
            public PassengerType PassengerType = PassengerType.Standard;
            public ColorType Color = ColorType.Red;
            public override string GetTypeName() => "Passenger";
        }

        [Serializable]
        public class TunnelContent : CellContent
        {
            [Tooltip("The color order of the passengers that will exit from the tunnel.")]
            public List<ColorType> PassengerSequence = new List<ColorType>();
            public override string GetTypeName() => "Tunnel";
        }
    
        // EMPTY CELL DATA (Usually represented only with Type.Empty, but included for consistency)
        [Serializable]
        public class EmptyContent : CellContent
        {
            public override string GetTypeName() => "Empty";
        }
}