using AirportAutomation.Core.Entities;

namespace AirportAutomation.UI.Services
{
    public class AuthContext
    {
        public User? CurrentUser { get; private set; }

        public void SetUser(User user)
        {
            CurrentUser = user;
        }

        public bool IsInRole(string role) =>
            CurrentUser != null &&
            string.Equals(CurrentUser.Role, role, StringComparison.OrdinalIgnoreCase);

        public bool IsStaffOrAdmin() =>
            IsInRole("Admin") || IsInRole("Staff");
    }
}

