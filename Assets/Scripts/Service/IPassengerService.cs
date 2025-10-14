using BusJamDemo.Grid;

namespace BusJamDemo.Service
{
    public interface IPassengerService
    {
        void ClearPassengers();
        void RegisterPassenger(Passenger passenger);
    }
}