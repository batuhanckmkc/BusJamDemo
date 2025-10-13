using TMPro;
using UnityEngine;

namespace BusJamDemo.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private StartScreenUI startScreen;
        [SerializeField] private GameplayScreenUI gameplayScreen;
        [SerializeField] private EndGameScreenUI endGameScreen;

        public StartScreenUI StartScreenUI => startScreen;
        public GameplayScreenUI GameplayScreenUI => gameplayScreen;
        public EndGameScreenUI EndGameScreenUI => endGameScreen;
        public static UIManager Instance;
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
    }
}