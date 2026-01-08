using AirportAutomation.Interface;
using AirportAutomation.UI.Helpers;

namespace AirportAutomation.UI.Pages;

public partial class MainPage : ContentPage
{
    private readonly IFlightService _flightService;
    private bool _initialized;

    public MainPage() : this(ServiceHelper.GetRequiredService<IFlightService>())
    {
    }

    public MainPage(IFlightService flightService)
    {
        InitializeComponent();
        _flightService = flightService;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (!_initialized)
        {
            _initialized = true;
            _ = LoadFlightsAsync();
        }
    }

    private async Task LoadFlightsAsync()
    {
        await Task.Run(() =>
        {
            var hata = _flightService.GetAllFlights(out var flights);
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (!string.IsNullOrEmpty(hata))
                {
                    DisplayAlert("Hata", hata, "Tamam");
                    FlightsView.ItemsSource = Array.Empty<object>();
                }
                else
                {
                    FlightsView.ItemsSource = flights;
                }
            });
        });
    }

    private void OnRefreshClicked(object sender, EventArgs e)
    {
        _ = LoadFlightsAsync();
    }
}
