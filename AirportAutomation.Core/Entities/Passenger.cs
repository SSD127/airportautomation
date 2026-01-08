namespace AirportAutomation.Core.Entities
{
    public class Passenger
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PnrCode { get; set; } = string.Empty;
        public string FlightNumber { get; set; } = string.Empty;
        public string SeatNumber { get; set; } = string.Empty;
        public bool IsCheckedIn { get; set; }
        public string LoyaltyTier { get; set; } = "Standard";
    }
}

