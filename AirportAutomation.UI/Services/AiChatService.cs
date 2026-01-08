using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AirportAutomation.UI.Models;

namespace AirportAutomation.UI.Services;

public class AiChatService
{
    // Gemini generateContent: önce v1 (genelde açık), 404 olursa v1beta; pro fallback
    private static readonly string[] Endpoints =
    {
        "https://generativelanguage.googleapis.com/v1/models/gemini-1.5-flash-latest:generateContent",
        "https://generativelanguage.googleapis.com/v1/models/gemini-pro:generateContent",
        "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent",
        "https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent"
    };

    private readonly ApiSettings _settings;
    private readonly HttpClient _httpClient = new();

    public AiChatService(ApiSettings settings)
    {
        _settings = settings;
    }

    public async Task<(string hata, string yanit)> ChatAsync(string prompt, string? endpointOverride = null)
    {
        if (string.IsNullOrWhiteSpace(_settings.ApiKey))
            return ("API anahtarı bulunamadı.", string.Empty);

        var tried = new List<string>();
        var endpoints = string.IsNullOrWhiteSpace(endpointOverride)
            ? Endpoints
            : new[] { endpointOverride };

        foreach (var ep in endpoints)
        {
            var url = ep.Contains("?") ? $"{ep}&key={_settings.ApiKey}" : $"{ep}?key={_settings.ApiKey}";
            tried.Add(ep);
            try
            {
                using var req = new HttpRequestMessage(HttpMethod.Post, url);
                req.Headers.Accept.ParseAdd("application/json");

                var payload = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = prompt }
                            }
                        }
                    }
                };

                var json = JsonSerializer.Serialize(payload);
                req.Content = new StringContent(json, Encoding.UTF8, "application/json");

                using var res = await _httpClient.SendAsync(req);
                if (!res.IsSuccessStatusCode)
                {
                    // 404 ise sıradaki endpointi dene
                    if (res.StatusCode == System.Net.HttpStatusCode.NotFound)
                        continue;
                    var body = await res.Content.ReadAsStringAsync();
                    return ($"AI isteği başarısız: {(int)res.StatusCode} {res.ReasonPhrase}. İçerik: {body}", string.Empty);
                }

                var doc = await JsonDocument.ParseAsync(await res.Content.ReadAsStreamAsync());
                var root = doc.RootElement;
                if (!root.TryGetProperty("candidates", out var candidates) || candidates.GetArrayLength() == 0)
                    return ("AI yanıtı boş döndü.", string.Empty);

                var parts = candidates[0].GetProperty("content").GetProperty("parts");
                if (parts.GetArrayLength() == 0)
                    return ("AI yanıtı boş döndü.", string.Empty);

                var content = parts[0].GetProperty("text").GetString() ?? string.Empty;
                return (string.Empty, content.Trim());
            }
            catch (Exception ex)
            {
                return (ex.Message, string.Empty);
            }
        }

        return ($"AI isteği başarısız: denenen endpointler bulunamadı ({string.Join(", ", tried)})", string.Empty);
    }
}

