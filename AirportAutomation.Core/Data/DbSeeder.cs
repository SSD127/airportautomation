using AirportAutomation.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace AirportAutomation.Core.Data
{
    public static class DbSeeder
    {
        public static string Seed(AirportDbContext context)
        {
            string? hata = null;
            try
            {
                context.Database.EnsureCreated();

                // Kapı seed'i: yoksa ekle
                string[] gates = { "A1", "A2", "B1", "B2", "C1" };
                if (!context.Gates.Any())
                {
                    foreach (var code in gates)
                    {
                        context.Gates.Add(new Gate
                        {
                            Code = code,
                            Status = "Closed",
                            CurrentFlightNumber = string.Empty
                        });
                    }
                    context.SaveChanges();
                }

                if (!context.Flights.Any())
                {
                    string[] cities = { "London", "New York", "Tokyo", "Paris", "Berlin", "Dubai" };
                    string[] airlines = { "TK", "LH", "BA", "AA", "EK" };
                    string[] statuses = { "On Time", "Delayed", "Boarding" };

                    var rnd = new Random();

                    for (int i = 0; i < 50; i++)
                    {
                        var flight = new Flight
                        {
                            FlightNumber = airlines[rnd.Next(airlines.Length)] + rnd.Next(100, 999),
                            Origin = "Istanbul",
                            Destination = cities[rnd.Next(cities.Length)],
                            DepartureTime = DateTime.Now.AddHours(rnd.Next(-4, 24)),
                            GateCode = gates[rnd.Next(gates.Length)],
                            Status = statuses[rnd.Next(statuses.Length)],
                            Price = rnd.Next(100, 800)
                        };
                        context.Flights.Add(flight);
                    }

                    context.SaveChanges();

                    var flights = context.Flights.ToList();
                    string[] names = { "Ahmet", "Mehmet", "Ayşe", "Fatma", "John", "Jane" };
                    string[] surnames = { "Yılmaz", "Demir", "Kaya", "Smith", "Doe" };

                    for (int i = 0; i < 1000; i++)
                    {
                        var f = flights[rnd.Next(flights.Count)];
                        var pax = new Passenger
                        {
                            FullName = $"{names[rnd.Next(names.Length)]} {surnames[rnd.Next(surnames.Length)]}",
                            PnrCode = "PNR" + rnd.Next(1000, 9999),
                            FlightNumber = f.FlightNumber,
                            IsCheckedIn = rnd.Next(2) == 1,
                            SeatNumber = rnd.Next(1, 30) + (rnd.Next(2) == 1 ? "A" : "F"),
                            LoyaltyTier = rnd.Next(10) > 7 ? "Gold" : "Standard"
                        };
                        context.Passengers.Add(pax);
                    }

                    context.SaveChanges();
                }

                if (!context.Users.Any())
                {
                    var adminSalt = Helpers.PasswordHelper.CreateSalt();
                    context.Users.Add(new User
                    {
                        Username = "admin",
                        FullName = "System Admin",
                        Role = "Admin",
                        Salt = adminSalt,
                        PasswordHash = Helpers.PasswordHelper.HashPassword("admin123", adminSalt)
                    });

                    // 30 sahte personel (check-in vs.)
                    for (int i = 1; i <= 30; i++)
                    {
                        var staffSalt = Helpers.PasswordHelper.CreateSalt();
                        context.Users.Add(new User
                        {
                            Username = $"personel{i:D2}",
                            FullName = $"Personel {i}",
                            Role = "Staff",
                            Salt = staffSalt,
                            PasswordHash = Helpers.PasswordHelper.HashPassword("personel123", staffSalt)
                        });
                    }

                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                hata = ex.Message;
            }

            return hata ?? string.Empty;
        }
    }
}

