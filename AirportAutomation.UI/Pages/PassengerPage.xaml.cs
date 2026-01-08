using AirportAutomation.Interface;
using AirportAutomation.UI.Helpers;
using AirportAutomation.UI.Services;

namespace AirportAutomation.UI.Pages;

public partial class PassengerPage : ContentPage
{
    private readonly IPassengerService _passengerService;
    private readonly IFlightService _flightService;
    private readonly AuthContext _auth;

    public PassengerPage() : this(
        ServiceHelper.GetRequiredService<IPassengerService>(),
        ServiceHelper.GetRequiredService<IFlightService>(),
        ServiceHelper.GetRequiredService<AuthContext>())
    {
    }

    public PassengerPage(IPassengerService passengerService, IFlightService flightService, AuthContext auth)
    {
        InitializeComponent();
        _passengerService = passengerService;
        _flightService = flightService;
        _auth = auth;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        var isAdmin = _auth.IsInRole("Admin");
        AdminListFrame.IsVisible = isAdmin;
        if (isAdmin)
        {
            _ = LoadAllPassengersAsync();
        }
    }

    private void OnSearchClicked(object sender, EventArgs e)
    {
        var pnr = PnrEntry.Text?.Trim();
        if (string.IsNullOrWhiteSpace(pnr))
        {
            DisplayAlert("Uyarı", "PNR giriniz.", "Tamam");
            return;
        }

        var hata = _passengerService.GetPassengerByPnr(pnr, out var passenger);
        if (!string.IsNullOrEmpty(hata))
        {
            DisplayAlert("Hata", hata, "Tamam");
            return;
        }

        PassengerLabel.Text = $"{passenger.FullName} ({passenger.PnrCode})";
        StatusLabel.Text = passenger.IsCheckedIn ? $"Check-in yapılmış ({passenger.SeatNumber})" : "Check-in yapılmadı";

        if (string.IsNullOrEmpty(passenger.FlightNumber))
        {
            FlightLabel.Text = "-";
            return;
        }

        var fHata = _flightService.GetAllFlights(out var flights);
        if (!string.IsNullOrEmpty(fHata))
        {
            DisplayAlert("Hata", fHata, "Tamam");
            return;
        }

        var flight = flights.FirstOrDefault(f => f.FlightNumber == passenger.FlightNumber);
        if (flight != null)
        {
            FlightLabel.Text = $"{flight.FlightNumber} {flight.Origin} -> {flight.Destination} {flight.DepartureTime:dd.MM HH:mm} Kapı:{flight.GateCode}";
        }
    }

    private void OnCheckInClicked(object sender, EventArgs e)
    {
        var pnr = PnrEntry.Text?.Trim();
        var seat = SeatEntry.Text?.Trim();
        if (string.IsNullOrWhiteSpace(pnr) || string.IsNullOrWhiteSpace(seat))
        {
            DisplayAlert("Uyarı", "PNR ve koltuk giriniz.", "Tamam");
            return;
        }

        var hata = _passengerService.CheckIn(pnr, seat);
        if (!string.IsNullOrEmpty(hata))
        {
            DisplayAlert("Hata", hata, "Tamam");
            return;
        }

        DisplayAlert("Bilgi", "Check-in tamamlandı.", "Tamam");
        OnSearchClicked(sender, e);
    }

    private void OnHelpClicked(object sender, EventArgs e)
    {
        var pnr = PnrEntry.Text?.Trim();
        var msg = string.IsNullOrWhiteSpace(pnr)
            ? "Yardım talebiniz alındı. Personel en kısa sürede yardımcı olacak."
            : $"Yardım talebi iletildi. PNR: {pnr}. Personel en kısa sürede yardımcı olacak.";
        DisplayAlert("Yardım Çağrısı", msg, "Tamam");
    }

    private async Task LoadAllPassengersAsync()
    {
        await Task.Run(() =>
        {
            var hata = _passengerService.GetAllPassengers(out var passengers);
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (!string.IsNullOrEmpty(hata))
                {
                    DisplayAlert("Hata", hata, "Tamam");
                    PassengerView.ItemsSource = Array.Empty<object>();
                }
                else
                {
                    PassengerView.ItemsSource = passengers;
                }
            });
        });
    }

    private void OnAdminRefreshClicked(object sender, EventArgs e)
    {
        _ = LoadAllPassengersAsync();
    }
}

