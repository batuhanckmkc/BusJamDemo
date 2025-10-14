using System.Linq;
using BusJamDemo.LevelLoad;
using BusJamDemo.Service;
using UnityEngine;


namespace BusJamDemo.Core
{
    public class LevelManager : MonoBehaviour, ILevelService
    {
        private LevelData_SO[] _uniqueLevels;
        private LevelData_SO _currentLevelData;
        private int _currentLevelIndex;
        public LevelData_SO CurrentLevelData => _currentLevelData;
        public int CurrentLevelNumber => _currentLevelIndex + 1;
        private const string LevelSaveKey = "LastPlayedLevel";
        private const string ResourcesLevelsPath = "Levels";
        private IGameService _gameService;
        private ILevelLoader _loader;
        public void Initialize(IGameService gameService, ILevelLoader loader)
        {
            _gameService = gameService;
            _loader = loader;
            _gameService.OnGameStateChanged += OnGameStateChange;
        }

        public void ActivateSystem()
        {
            InitializeLevelIndex();
            LoadAllLevelsFromResources(); 
        }
        
        private void LoadAllLevelsFromResources()
        {
            _uniqueLevels = Resources.LoadAll<LevelData_SO>(ResourcesLevelsPath)
                .OrderBy(level => level.LevelIndex)
                .ToArray();
            
            if (_uniqueLevels.Length == 0)
            {
                Debug.LogError("LevelManager: LevelData_SO assets could not be loaded from Resources/Levels. Check file path and asset type.");
            }
        }
        
        private void OnGameStateChange(GameState gameState)
        {
            if (gameState == GameState.StartScreen)
            {
                LoadCurrentLevel();
            }
        }

        private void InitializeLevelIndex()
        {
            _currentLevelIndex = PlayerPrefs.GetInt("LastPlayedLevel", 0);
        }

        private void LoadCurrentLevel()
        {
            if (_uniqueLevels == null || _uniqueLevels.Length == 0)
            {
                Debug.LogError("LevelManager: Cannot load level, uniqueLevels array is empty.");
                return;
            }
            if (_currentLevelIndex >= _uniqueLevels.Length)
            {
                _currentLevelIndex = 0;
                PlayerPrefs.SetInt(LevelSaveKey, _currentLevelIndex);
            }
            _currentLevelData = _uniqueLevels[_currentLevelIndex];
            _loader.LoadLevel(_currentLevelData);
        }
        
        public void AdvanceToNextLevel()
        {
            _currentLevelIndex++;
            PlayerPrefs.SetInt(LevelSaveKey, _currentLevelIndex);
        }
    }
}