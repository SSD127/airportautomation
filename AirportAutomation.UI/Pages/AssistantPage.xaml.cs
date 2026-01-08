using AirportAutomation.Interface;
using AirportAutomation.Core.Entities;
using AirportAutomation.UI.Helpers;
using AirportAutomation.UI.Services;
using AirportAutomation.UI.Models;
using Microsoft.Maui.Storage;

namespace AirportAutomation.UI.Pages;

public partial class AssistantPage : ContentPage
{
    private readonly IAssistantService _assistantService;
    private readonly IPassengerService _passengerService;
    private readonly IFlightService _flightService;
    private readonly AuthContext _auth;
    private readonly ApiSettings _apiSettings;
    private readonly AiChatService _aiChat;
    private readonly List<ChatLog> _logs = new();
    private string _apiKey = string.Empty;
    private string _endpoint = string.Empty;

    public AssistantPage() : this(
        ServiceHelper.GetRequiredService<IAssistantService>(),
        ServiceHelper.GetRequiredService<IPassengerService>(),
        ServiceHelper.GetRequiredService<IFlightService>(),
        ServiceHelper.GetRequiredService<AuthContext>(),
        ServiceHelper.GetRequiredService<ApiSettings>(),
        ServiceHelper.GetRequiredService<AiChatService>())
    {
    }

    public AssistantPage(IAssistantService assistantService, IPassengerService passengerService, IFlightService flightService, AuthContext auth, ApiSettings apiSettings, AiChatService aiChat)
    {
        InitializeComponent();
        _assistantService = assistantService;
        _passengerService = passengerService;
        _flightService = flightService;
        _auth = auth;
        _apiSettings = apiSettings;
        _aiChat = aiChat;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Varsayılan: config’ten gelen anahtar; çevre değişkeni varsa onu kullan.
        _apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY") ??
                  _apiSettings.ApiKey;
        // Opsiyonel endpoint; burada boş bırakıyoruz (service default kullanır).
        _endpoint = string.Empty;
    }

    private async void OnSendClicked(object sender, EventArgs e)
    {
        if (SendButton.IsEnabled == false) return;
        var question = QuestionEntry.Text?.Trim();
        if (string.IsNullOrWhiteSpace(question))
        {
            StatusLabel.Text = "Lütfen soru yazın.";
            return;
        }

        // Varsayılan anahtar boşsa hata ver.
        if (string.IsNullOrWhiteSpace(_apiKey))
        {
            StatusLabel.Text = "API anahtarı bulunamadı.";
            return;
        }

        SendButton.IsEnabled = false;
        StatusLabel.Text = "Gönderiliyor...";

        var (hata, yanit) = await _aiChat.ChatAsync(question, string.IsNullOrWhiteSpace(_endpoint) ? null : _endpoint);
        if (!string.IsNullOrEmpty(hata))
        {
            StatusLabel.Text = "AI yanıtı alınamadı.";
            await DisplayAlert("Hata", hata, "Tamam");
            SendButton.IsEnabled = true;
            return;
        }

        _logs.Add(new ChatLog
        {
            UserMessage = question,
            AiResponse = yanit,
            CreatedAt = DateTime.Now
        });
        ChatView.ItemsSource = null;
        ChatView.ItemsSource = _logs.ToList();
        QuestionEntry.Text = string.Empty;
        StatusLabel.Text = "Hazır";
        SendButton.IsEnabled = true;
    }

    private string ExtractDemoPnr(string text)
    {
        // Basit: PNR01 gibi bir kod geçerse onu yakala
        if (text == null) return string.Empty;
        var parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var pnr = parts.FirstOrDefault(p => p.StartsWith("PNR", StringComparison.OrdinalIgnoreCase));
        return pnr ?? string.Empty;
    }
}

