using AirportAutomation.Core.Entities;

namespace AirportAutomation.Interface
{
    public interface IFlightService
    {
        string GetAllFlights(out List<Flight> flights);
    }
}

