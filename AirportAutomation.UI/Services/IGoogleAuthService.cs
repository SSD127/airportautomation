using AirportAutomation.UI.Models;

namespace AirportAutomation.UI.Services
{
    public interface IGoogleAuthService
    {
        Task<(string hata, GoogleAuthResult? result)> LoginAsync();
    }
}

