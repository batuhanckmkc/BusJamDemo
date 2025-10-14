using BusJamDemo.LevelLoad;
using UnityEngine;

namespace BusJamDemo.Service
{
    public interface IPoolService
    {
        void PreloadLevelAssets(LevelData_SO levelData);
        T Get<T>() where T : Component;
        void Release<T>(T item) where T : Component;
        void ClearAllPools();
    }
}