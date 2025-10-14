using BusJamDemo.Service;
using TMPro;
using UnityEngine;

namespace BusJamDemo.UI
{
    public class GameplayScreenUI : ScreenUI
    {
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI timerText;

        private ITimerService _timerService;
        private ILevelService _levelService;
        public void Initialize(ITimerService timerService, ILevelService levelService)
        {
            _timerService = timerService;
            _levelService = levelService;
            _timerService.OnTimerUpdate += OnTimerUpdate;
        }
        public void UpdateLevelDisplay(int level)
        {
            levelText.text = "Level " + level;
        }

        private void OnTimerUpdate(float time)
        {
            timerText.text = ((int)time).ToString();
        }

        public override void Show()
        {
            UpdateLevelDisplay(_levelService.CurrentLevelData.LevelIndex);
            base.Show();
        }
    }
}