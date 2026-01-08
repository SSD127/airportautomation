using AirportAutomation.Interface;
using AirportAutomation.Core.Entities;
using AirportAutomation.UI.Helpers;
using AirportAutomation.UI.Services;

namespace AirportAutomation.UI.Pages;

public partial class GatePage : ContentPage
{
    private readonly IGateService _gateService;
    private readonly IFlightService _flightService;
    private readonly AuthContext _auth;
    private bool _initialized;
    private Gate? _selectedGate;
    private List<Flight> _flights = new();

    public GatePage() : this(
        ServiceHelper.GetRequiredService<IGateService>(),
        ServiceHelper.GetRequiredService<IFlightService>(),
        ServiceHelper.GetRequiredService<AuthContext>())
    {
    }

    public GatePage(IGateService gateService, IFlightService flightService, AuthContext auth)
    {
        InitializeComponent();
        _gateService = gateService;
        _flightService = flightService;
        _auth = auth;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (!_auth.IsStaffOrAdmin())
        {
            DisplayAlert("Yetki", "Bu sayfa için personel ya da admin rolü gerekli.", "Tamam");
            Shell.Current?.GoToAsync("//MainPage");
            return;
        }
        if (!_initialized)
        {
            _initialized = true;
            _ = LoadAsync();
        }
    }

    private async Task LoadAsync()
    {
        await Task.WhenAll(LoadGatesAsync(), LoadFlightsAsync());
    }

    private async Task LoadGatesAsync()
    {
        await Task.Run(() =>
        {
            var hata = _gateService.GetAllGates(out var gates);
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (!string.IsNullOrEmpty(hata))
                {
                    DisplayAlert("Hata", hata, "Tamam");
                    GateView.ItemsSource = Array.Empty<object>();
                }
                else
                {
                    GateView.ItemsSource = gates;
                }
            });
        });
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
                    FlightPicker.ItemsSource = Array.Empty<object>();
                }
                else
                {
                    _flights = flights;
                    FlightPicker.ItemsSource = flights;
                }
            });
        });
    }

    private void OnRefreshClicked(object sender, EventArgs e)
    {
        _ = LoadAsync();
    }

    private void OnGateSelected(object sender, SelectionChangedEventArgs e)
    {
        _selectedGate = e.CurrentSelection.FirstOrDefault() as Gate;
        if (_selectedGate != null)
        {
            SelectedGateLabel.Text = $"{_selectedGate.Code} | {_selectedGate.Status} | {_selectedGate.CurrentFlightNumber}";
        }
        else
        {
            SelectedGateLabel.Text = "-";
        }
    }

    private void OnToggleGateClicked(object sender, EventArgs e)
    {
        if (_selectedGate == null)
        {
            DisplayAlert("Uyarı", "Önce bir kapı seçin.", "Tamam");
            return;
        }

        var newStatus = _selectedGate.Status == "Open" ? "Closed" : "Open";
        var hata = _gateService.UpdateGateStatus(_selectedGate.Code, newStatus, _selectedGate.CurrentFlightNumber);
        if (!string.IsNullOrEmpty(hata))
        {
            DisplayAlert("Hata", hata, "Tamam");
            return;
        }

        _ = LoadGatesAsync();
    }

    private void OnAssignFlightClicked(object sender, EventArgs e)
    {
        if (_selectedGate == null)
        {
            DisplayAlert("Uyarı", "Önce bir kapı seçin.", "Tamam");
            return;
        }

        if (FlightPicker.SelectedItem is not Flight flight)
        {
            DisplayAlert("Uyarı", "Bir uçuş seçin.", "Tamam");
            return;
        }

        var hata = _gateService.UpdateGateStatus(_selectedGate.Code, "Open", flight.FlightNumber);
        if (!string.IsNullOrEmpty(hata))
        {
            DisplayAlert("Hata", hata, "Tamam");
            return;
        }

        _ = LoadGatesAsync();
    }

    private void OnClearFlightClicked(object sender, EventArgs e)
    {
        if (_selectedGate == null)
        {
            DisplayAlert("Uyarı", "Önce bir kapı seçin.", "Tamam");
            return;
        }

        var hata = _gateService.UpdateGateStatus(_selectedGate.Code, "Closed", string.Empty);
        if (!string.IsNullOrEmpty(hata))
        {
            DisplayAlert("Hata", hata, "Tamam");
            return;
        }

        _ = LoadGatesAsync();
    }
}

