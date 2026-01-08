using AirportAutomation.Core.Data;
using AirportAutomation.Core.Entities;
using AirportAutomation.Interface;
using Microsoft.EntityFrameworkCore;

namespace AirportAutomation.Service
{
    public class SFlightService : IFlightService
    {
        public string GetAllFlights(out List<Flight> flights)
        {
            string? hata = null;
            flights = new List<Flight>();

            try
            {
                using var context = new AirportDbContext();
                flights = context.Flights.AsNoTracking().OrderBy(f => f.DepartureTime).ToList();
            }
            catch (Exception ex)
            {
                hata = ex.Message;
            }

            return hata ?? string.Empty;
        }
    }
}

