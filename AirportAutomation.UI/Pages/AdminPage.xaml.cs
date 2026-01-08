using AirportAutomation.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Storage;
using AirportAutomation.UI.Helpers;
using AirportAutomation.UI.Services;
using AirportAutomation.UI.Models;

namespace AirportAutomation.UI.Pages;

public partial class AdminPage : ContentPage
{
    private readonly AirportDbContext _context;
    private readonly AuthContext _auth;
    private readonly ApiSettings _apiSettings;
    private readonly AiChatService _aiChat;

    public AdminPage() : this(
        ServiceHelper.GetRequiredService<AirportDbContext>(),
        ServiceHelper.GetRequiredService<AuthContext>(),
        ServiceHelper.GetRequiredService<ApiSettings>(),
        ServiceHelper.GetRequiredService<AiChatService>())
    {
    }

    public AdminPage(AirportDbContext context, AuthContext auth, ApiSettings apiSettings, AiChatService aiChat)
    {
        InitializeComponent();
        _context = context;
        _auth = auth;
        _apiSettings = apiSettings;
        _aiChat = aiChat;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (!_auth.IsInRole("Admin"))
        {
            DisplayAlert("Yetki", "Bu sayfa için admin rolü gerekli.", "Tamam");
            Shell.Current?.GoToAsync("//MainPage");
            return;
        }
        LoadAiSettings();
        _ = LoadAsync();
    }

    private async Task LoadAsync()
    {
        var flights = await _context.Flights.ToListAsync();
        var passengers = await _context.Passengers.ToListAsync();

        FlightCountLabel.Text = flights.Count.ToString();
        PassengerCountLabel.Text = passengers.Count.ToString();

        var revenue = (from p in passengers
                       join f in flights on p.FlightNumber equals f.FlightNumber
                       select f.Price).Sum();
        RevenueLabel.Text = revenue.ToString("N0");

        var popular = flights
            .GroupBy(f => f.Destination)
            .Select(g => new { Key = g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .Take(5)
            .ToList();
        PopularView.ItemsSource = popular;
    }

    private void OnRefreshClicked(object sender, EventArgs e)
    {
        _ = LoadAsync();
    }

    private void LoadAiSettings()
    {
        AiSettingsStatus.Text = string.IsNullOrWhiteSpace(_apiSettings.ApiKey)
            ? "Varsayılan anahtar bulunamadı. API çağrısı başarısız olabilir."
            : "Varsayılan anahtar yüklendi; ek giriş gerekmiyor.";
    }

    private async void OnSendAiClicked(object sender, EventArgs e)
    {
        AiResponseLabel.Text = "Yanıt bekleniyor...";
        var prompt = AiPromptEditor.Text?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(prompt))
        {
            AiResponseLabel.Text = "Lütfen soru yazın.";
            return;
        }

        var (hata, yanit) = await _aiChat.ChatAsync(prompt);
        AiResponseLabel.Text = string.IsNullOrEmpty(hata) ? yanit : $"Hata: {hata}";
    }
}

