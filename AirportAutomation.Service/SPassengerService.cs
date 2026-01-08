using AirportAutomation.Core.Data;
using AirportAutomation.Core.Entities;
using AirportAutomation.Interface;
using Microsoft.EntityFrameworkCore;

namespace AirportAutomation.Service
{
    public class SPassengerService : IPassengerService
    {
        public string GetAllPassengers(out List<Passenger> passengers)
        {
            string? hata = null;
            passengers = new List<Passenger>();

            try
            {
                using var context = new AirportDbContext();
                passengers = context.Passengers.AsNoTracking().OrderBy(p => p.PnrCode).ToList();
            }
            catch (Exception ex)
            {
                hata = ex.Message;
            }

            return hata ?? string.Empty;
        }

        public string GetPassengerByPnr(string pnr, out Passenger passenger)
        {
            string? hata = null;
            passenger = new Passenger();

            try
            {
                using var context = new AirportDbContext();
                passenger = context.Passengers.AsNoTracking().FirstOrDefault(p => p.PnrCode == pnr) ?? new Passenger();
            }
            catch (Exception ex)
            {
                hata = ex.Message;
            }

            return hata ?? string.Empty;
        }

        public string CheckIn(string pnr, string seat)
        {
            string? hata = null;

            try
            {
                using var context = new AirportDbContext();
                var pax = context.Passengers.FirstOrDefault(p => p.PnrCode == pnr);
                if (pax == null)
                {
                    return "Yolcu bulunamadı.";
                }

                pax.SeatNumber = seat;
                pax.IsCheckedIn = true;
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

