using AirportAutomation.Interface;
using AirportAutomation.Core.Entities;
using AirportAutomation.UI.Helpers;

namespace AirportAutomation.UI.Pages;

public partial class BookingPage : ContentPage
{
    private readonly IBookingService _bookingService;
    private readonly IFlightService _flightService;

    public BookingPage() : this(
        ServiceHelper.GetRequiredService<IBookingService>(),
        ServiceHelper.GetRequiredService<IFlightService>())
    {
    }

    public BookingPage(IBookingService bookingService, IFlightService flightService)
    {
        InitializeComponent();
        _bookingService = bookingService;
        _flightService = flightService;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadFlights();
    }

    private void LoadFlights()
    {
        var hata = _bookingService.GetUpcomingFlights(out var flights);
        if (!string.IsNullOrEmpty(hata))
        {
            DisplayAlert("Hata", hata, "Tamam");
            return;
        }

        FlightPicker.ItemsSource = flights;
    }

    private void OnBookClicked(object sender, EventArgs e)
    {
        var pnr = SourcePnrEntry.Text?.Trim();
        if (string.IsNullOrWhiteSpace(pnr))
        {
            DisplayAlert("Uyarı", "Kaynak PNR giriniz.", "Tamam");
            return;
        }

        if (FlightPicker.SelectedItem is not Flight flight)
        {
            DisplayAlert("Uyarı", "Bir uçuş seçiniz.", "Tamam");
            return;
        }

        var hata = _bookingService.BookTicket(pnr, flight.FlightNumber, out var ticket);
        if (!string.IsNullOrEmpty(hata))
        {
            DisplayAlert("Hata", hata, "Tamam");
            return;
        }

        TicketLabel.Text = $"Yeni PNR: {ticket.PnrCode} | {ticket.FlightNumber}";
    }
}

