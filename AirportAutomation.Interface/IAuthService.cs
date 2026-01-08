using AirportAutomation.Core.Entities;

namespace AirportAutomation.Interface
{
    public interface IAuthService
    {
        string RegisterPassenger(string username, string password, string fullName, out User user);
        string Login(string username, string password, out User user);
        string EnsureExternalPassenger(string username, string fullName, out User user);
    }
}

