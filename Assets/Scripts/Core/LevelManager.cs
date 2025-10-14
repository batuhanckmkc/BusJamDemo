using BusJamDemo.LevelLoad;
using BusJamDemo.Service;
using UnityEngine;


namespace BusJamDemo.Core
{
    public class LevelManager : MonoBehaviour, ILevelService
    {
        [SerializeField] private LevelData_SO[] uniqueLevels;

        private IGameService _gameService;
        private ILevelLoader _loader;
        
        
        private LevelData_SO _currentLevelData;
        private int _currentLevelIndex;
        public LevelData_SO CurrentLevelData => _currentLevelData;
        public int CurrentLevelNumber => _currentLevelIndex + 1;
        private const string LevelSaveKey = "LastPlayedLevel";


        public void Initialize(IGameService gameService, ILevelLoader loader)
        {
            _gameService = gameService;
            _loader = loader;
            
            _gameService.OnGameStateChanged += OnGameStateChange;

        }

        public void ActivateSystem()
        {
            InitializeLevelIndex();
            // LoadCurrentLevel();
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
            if (_currentLevelIndex >= uniqueLevels.Length)
            {
                _currentLevelIndex = 0;
                PlayerPrefs.SetInt(LevelSaveKey, _currentLevelIndex);
            }
            _currentLevelData = uniqueLevels[_currentLevelIndex];
            _loader.LoadLevel(_currentLevelData);
        }
        
        public void AdvanceToNextLevel()
        {
            _currentLevelIndex++;
            PlayerPrefs.SetInt(LevelSaveKey, _currentLevelIndex);
        }
    }
}