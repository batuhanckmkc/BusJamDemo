using BusJamDemo.Core;
using BusJamDemo.Service;
using UnityEngine;
using UnityEngine.UI;

namespace BusJamDemo.UI
{
    public class StartScreenUI : ScreenUI
    {
        [SerializeField] private Button startButton;

        private IGameService _gameService;
        public void Initialize(IGameService gameService)
        {
            _gameService = gameService;
        }
        
        private void OnEnable()
        {
            startButton.onClick.AddListener(HandleTapToStart);
        }

        private void OnDisable()
        {
            startButton.onClick.RemoveListener(HandleTapToStart);
        }

        private void HandleTapToStart()
        {
            _gameService.UpdateGameState(GameState.Gameplay);
            Hide();
        }
    }
}