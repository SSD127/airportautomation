namespace AirportAutomation.Core.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Salt { get; set; } = string.Empty;
        public string Role { get; set; } = "Passenger"; // Passenger, Staff, Admin
        public string FullName { get; set; } = string.Empty;
    }
}

