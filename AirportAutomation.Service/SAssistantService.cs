using AirportAutomation.Core.Data;
using AirportAutomation.Core.Entities;
using AirportAutomation.Interface;
using System.Net.Http.Json;
using System.Text.Json;

namespace AirportAutomation.Service
{
    public class SAssistantService : IAssistantService
    {
        private const string DefaultEndpoint = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent";

        public string AskQuestion(string question, string pnr, string apiKey, string? endpoint, out ChatLog chatLog)
        {
            string? hata = null;
            chatLog = new ChatLog();

            try
            {
                if (string.IsNullOrWhiteSpace(question)) return "Soru boş olamaz.";
                if (string.IsNullOrWhiteSpace(apiKey)) return "API anahtarı gerekli.";

                string responseText = string.Empty;

                // Basit Gemini isteği (endpoint admin ayarıyla değişebilir)
                var body = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = question }
                            }
                        }
                    }
                };

                var targetEndpoint = string.IsNullOrWhiteSpace(endpoint) ? DefaultEndpoint : endpoint;

                using (var client = new HttpClient())
                {
                    var url = $"{targetEndpoint}?key={apiKey}";
                    var res = client.PostAsJsonAsync(url, body).Result;
                    if (!res.IsSuccessStatusCode)
                    {
                        return $"API hatası: {res.StatusCode}";
                    }

                    using var doc = JsonDocument.Parse(res.Content.ReadAsStringAsync().Result);
                    responseText = doc.RootElement
                        .GetProperty("candidates")[0]
                        .GetProperty("content")
                        .GetProperty("parts")[0]
                        .GetProperty("text")
                        .GetString() ?? string.Empty;
                }

                using var context = new AirportDbContext();

                chatLog = new ChatLog
                {
                    UserMessage = question,
                    AiResponse = responseText,
                    CreatedAt = DateTime.UtcNow
                };
                context.ChatLogs.Add(chatLog);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                hata = ex.Message;
            }

            return hata ?? string.Empty;
        }
    }
}

