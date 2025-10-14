using System;

namespace BusJamDemo.Service
{
    public interface ITimerService
    {
        float CurrentTime { get; }
        void StartTimer(float duration);
        void StopTimer();
        Action<float> OnTimerUpdate { get; set; }
    }
}