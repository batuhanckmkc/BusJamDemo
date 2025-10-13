using BusJamDemo.LevelLoad;
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

        private const string LevelSaveKey = "LastPlayedLevel";
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
        }

        private void Start()
        {
            InitializeLevelIndex();
            LoadCurrentLevel();
        }

        private void OnEnable()
        {
            GameManager.OnGameStateChanged += OnGameStateChange;
        }

        private void OnDisable()
        {
            GameManager.OnGameStateChanged -= OnGameStateChange;
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
            levelLoader.LoadLevel(_currentLevelData);
        }
        
        public void AdvanceToNextLevel()
        {
            _currentLevelIndex++;
            PlayerPrefs.SetInt(LevelSaveKey, _currentLevelIndex);
        }
    }
}