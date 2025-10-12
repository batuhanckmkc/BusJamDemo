using BusJamDemo.LevelLoad;
using UnityEngine;

namespace BusJamDemo.Utility
{
    public static class Extensions
    {
        public static Color GetColor(this ColorType colorType)
        {
            switch (colorType)
            {
                case ColorType.Green:
                    return Color.green;
                case ColorType.Blue:
                    return Color.blue;
                case ColorType.Red:
                    return Color.red;
                case ColorType.Yellow:
                    return Color.yellow;
            }
            return new Color();
        }
    }
}