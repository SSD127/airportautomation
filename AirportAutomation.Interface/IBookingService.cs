using AirportAutomation.Core.Entities;

namespace AirportAutomation.Interface
{
    public interface IBookingService
    {
        string GetUpcomingFlights(out List<Flight> flights);
        string BookTicket(string pnrSource, string flightNumber, out Passenger newTicket);
    }
}

