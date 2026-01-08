using System.IO;
using Microsoft.EntityFrameworkCore;
using AirportAutomation.Core.Entities;

namespace AirportAutomation.Core.Data
{
    public class AirportDbContext : DbContext
    {
        public DbSet<Flight> Flights => Set<Flight>();
        public DbSet<Passenger> Passengers => Set<Passenger>();
        public DbSet<Gate> Gates => Set<Gate>();
        public DbSet<ChatLog> ChatLogs => Set<ChatLog>();
        public DbSet<User> Users => Set<User>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string localData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string dbDirectory = Path.Combine(localData, "AirportAutomation");
            Directory.CreateDirectory(dbDirectory);
            string dbPath = Path.Combine(dbDirectory, "airport.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Gate>().HasKey(g => g.Code);
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
        }
    }
}

