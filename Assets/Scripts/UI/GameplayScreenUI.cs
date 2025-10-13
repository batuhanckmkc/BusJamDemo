using BusJamDemo.Core;
using TMPro;
using UnityEngine;

namespace BusJamDemo.UI
{
    public class GameplayScreenUI : ScreenUI
    {
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI timerText;

        public void UpdateLevelDisplay(int level)
        {
            levelText.text = "Level " + level;
        }
        
        private void Update()
        {
            timerText.text = ((int)TimerManager.Instance.CurrentTime).ToString();
        }
    }
}