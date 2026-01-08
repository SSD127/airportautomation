using AirportAutomation.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace AirportAutomation.Core.Services
{
    public class SimulationService
    {
        private readonly Random _random = new Random();

        public string SimulateOnce()
        {
            string? hata = null;

            try
            {
                using var context = new AirportDbContext();
                var flights = context.Flights.Where(f => f.DepartureTime > DateTime.Now).ToList();
                if (!flights.Any())
                {
                    return string.Empty;
                }

                var flight = flights[_random.Next(flights.Count)];
                int action = _random.Next(10);

                if (action < 3)
                {
                    flight.Status = "Delayed";
                    flight.DepartureTime = flight.DepartureTime.AddMinutes(_random.Next(15, 120));
                }
                else if (action < 5)
                {
                    string[] gates = { "A1", "A2", "B1", "B2", "C1" };
                    var newGate = gates[_random.Next(gates.Length)];
                    flight.GateCode = newGate;

                    var gate = context.Gates.FirstOrDefault(g => g.Code == newGate);
                    if (gate != null)
                    {
                        gate.Status = "Closed";
                        gate.CurrentFlightNumber = string.Empty;
                    }
                }
                else
                {
                    return string.Empty;
                }

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

