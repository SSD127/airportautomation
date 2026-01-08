namespace AirportAutomation.Core.Entities
{
    public class Flight
    {
        public int Id { get; set; }
        public string FlightNumber { get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public string GateCode { get; set; } = string.Empty;
        public string Status { get; set; } = "On Time";
        public decimal Price { get; set; }
    }
}

