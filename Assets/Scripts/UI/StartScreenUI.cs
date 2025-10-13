using BusJamDemo.Core;
using UnityEngine;
using UnityEngine.UI;

namespace BusJamDemo.UI
{
    public class StartScreenUI : ScreenUI
    {
        [SerializeField] private Button startButton;

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
            GameManager.Instance.UpdateGameState(GameState.Gameplay);
            Hide();
        }
    }
}