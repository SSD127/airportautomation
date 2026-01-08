using AirportAutomation.Core.Entities;

namespace AirportAutomation.Interface
{
    public interface IAssistantService
    {
        string AskQuestion(string question, string pnr, string apiKey, string? endpoint, out ChatLog chatLog);
    }
}

