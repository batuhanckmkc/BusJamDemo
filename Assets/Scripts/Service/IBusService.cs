using System.Collections.Generic;
using BusJamDemo.BusSystem;
using BusJamDemo.LevelLoad;
using UnityEngine;

namespace BusJamDemo.Service
{
    public interface IBusService
    {
        Bus CurrentBus { get; }
        Vector3 StopPosition { get; }
        void CreateBuses(List<BusContent> busContents);
        void ClearBuses();
    }
}