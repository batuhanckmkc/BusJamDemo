using BusJamDemo.Grid;
using BusJamDemo.LevelLoad;
using UnityEngine;

namespace BusJamDemo.Service
{
    public interface ICellItemSpawner
    {
        Passenger SpawnPassenger(PassengerContent content, int row, int col, Transform parent = null);
        Tunnel SpawnTunnel(TunnelContent content, int row, int col, Transform parent = null);
    }
}