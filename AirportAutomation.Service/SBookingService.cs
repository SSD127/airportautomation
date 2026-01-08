using AirportAutomation.Core.Data;
using AirportAutomation.Core.Entities;
using AirportAutomation.Interface;
using Microsoft.EntityFrameworkCore;

namespace AirportAutomation.Service
{
    public class SBookingService : IBookingService
    {
        public string GetUpcomingFlights(out List<Flight> flights)
        {
            string? hata = null;
            flights = new List<Flight>();

            try
            {
                using var context = new AirportDbContext();
                flights = context.Flights.AsNoTracking()
                    .Where(f => f.DepartureTime > DateTime.Now)
                    .OrderBy(f => f.DepartureTime)
                    .Take(50)
                    .ToList();
            }
            catch (Exception ex)
            {
                hata = ex.Message;
            }

            return hata ?? string.Empty;
        }

        public string BookTicket(string pnrSource, string flightNumber, out Passenger newTicket)
        {
            string? hata = null;
            newTicket = new Passenger();

            try
            {
                using var context = new AirportDbContext();

                var flight = context.Flights.FirstOrDefault(f => f.FlightNumber == flightNumber);
                if (flight == null)
                {
                    return "Uçuş bulunamadı.";
                }

                // Kaynak PNR’dan yolcu bilgisi alınıp yeni bilet üretelim
                var source = context.Passengers.AsNoTracking().FirstOrDefault(p => p.PnrCode == pnrSource);
                if (source == null)
                {
                    return "Kaynak PNR bulunamadı.";
                }

                newTicket = new Passenger
                {
                    FullName = source.FullName,
                    PnrCode = "PNR" + new Random().Next(10000, 99999),
                    FlightNumber = flight.FlightNumber,
                    LoyaltyTier = source.LoyaltyTier,
                    IsCheckedIn = false,
                    SeatNumber = string.Empty
                };

                context.Passengers.Add(newTicket);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                hata = ex.Message;
            }

            return hata ?? string.Empty;
        }
    }
}

