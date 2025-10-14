using System.Collections.Generic;
using System.IO;
using BusJamDemo.Core;
using UnityEngine;

namespace BusJamDemo.Utility
{
    [System.Serializable]
    public class IntListWrapper
    {
        public List<int> values;
    }

    public static class IntListFileSaver
    {
        private static readonly string FilePath = Path.Combine(Application.persistentDataPath, "movedPassengers.json");

        public static void SaveIntList(List<int> list)
        {
            var wrapper = new IntListWrapper { values = list };
            string json = JsonUtility.ToJson(wrapper);
            File.WriteAllText(FilePath, json);
        }

        public static List<int> LoadIntList()
        {
            if (!File.Exists(FilePath))
                return new List<int>();

            string json = File.ReadAllText(FilePath);
            var wrapper = JsonUtility.FromJson<IntListWrapper>(json);
            return wrapper?.values ?? new List<int>();
        }
        
        public static void DeleteIntListFile()
        {
            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
                GameManager.ResumeGame = false;
                Debug.Log("Delete success!");
            }
        }

        public static bool HasData()
        {
            if (File.Exists(FilePath))
            {
                var loadedData = LoadIntList();
                return loadedData.Count > 0;
            }
            return false;
        }
    }
}