namespace AirportAutomation.Core.Entities
{
    public class Gate
    {
        public string Code { get; set; } = string.Empty;
        public string Status { get; set; } = "Closed";
        public string CurrentFlightNumber { get; set; } = string.Empty;
    }
}

