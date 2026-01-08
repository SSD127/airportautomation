using AirportAutomation.Core.Entities;

namespace AirportAutomation.Interface
{
    public interface IPassengerService
    {
        string GetPassengerByPnr(string pnr, out Passenger passenger);
        string CheckIn(string pnr, string seat);
        string GetAllPassengers(out List<Passenger> passengers);
    }
}

