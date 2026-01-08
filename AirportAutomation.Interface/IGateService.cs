using AirportAutomation.Core.Entities;

namespace AirportAutomation.Interface
{
    public interface IGateService
    {
        string GetAllGates(out List<Gate> gates);
        string UpdateGateStatus(string code, string status, string currentFlightNumber);
    }
}

