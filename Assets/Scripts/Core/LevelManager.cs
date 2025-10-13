using BusJamDemo.LevelLoad;
using BusJamDemo.UI;
using UnityEngine;


namespace BusJamDemo.Core
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private LevelData_SO[] uniqueLevels;
        [SerializeField] private LevelLoader levelLoader;

        private LevelData_SO _currentLevelData;
        public LevelData_SO CurrentLevelData => _currentLevelData;
        private int _currentLevelIndex;
        public int CurrentLevelNumber => _currentLevelIndex + 1;
        public static LevelManager Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            InitializeLevelIndex();
            LoadCurrentLevel();
        }

        private void OnEnable()
        {
            GameManager.OnGameStateChanged += OnGameStateChanged;
        }

        private void OnDisable()
        {
            GameManager.OnGameStateChanged -= OnGameStateChanged;
        }

        private void OnGameStateChanged(GameState newState) { }

        private void InitializeLevelIndex()
        {
            _currentLevelIndex = PlayerPrefs.GetInt("LastPlayedLevel", 0);
        }

        private void LoadCurrentLevel()
        {
            if (_currentLevelIndex >= uniqueLevels.Length)
            {
                GameManager.Instance.UpdateGameState(GameState.GameComplete);
                return;
            }
            _currentLevelData = uniqueLevels[_currentLevelIndex];
            levelLoader.LoadLevel(_currentLevelData);
        }

        public void AdvanceToNextLevel()
        {
            _currentLevelIndex++;

            PlayerPrefs.SetInt("LastPlayedLevel", _currentLevelIndex);
            PlayerPrefs.Save();
            if (_currentLevelIndex < uniqueLevels.Length)
            {
                GameManager.Instance.UpdateGameState(GameState.Gameplay);
            }
            else
            {
                GameManager.Instance.UpdateGameState(GameState.GameComplete);
            }
        }
    }
}