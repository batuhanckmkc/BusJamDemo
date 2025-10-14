using BusJamDemo.LevelLoad;

namespace BusJamDemo.Service
{
    public interface ILevelService
    {
        int CurrentLevelNumber { get; }
        LevelData_SO CurrentLevelData { get; }
        void AdvanceToNextLevel();
    }
}