namespace AirportAutomation.UI.Pages;

public partial class MapPage : ContentPage
{
    private readonly List<LocationItem> _locations = new()
    {
        new LocationItem { Name = "Giriş", Description = "Terminal girişi" },
        new LocationItem { Name = "Check-In", Description = "Bilet ve bagaj işlemleri" },
        new LocationItem { Name = "Güvenlik", Description = "Güvenlik kontrolü" },
        new LocationItem { Name = "Duty Free", Description = "Alışveriş alanı" },
        new LocationItem { Name = "Gate A1", Description = "Kalkış kapısı A1" },
        new LocationItem { Name = "Gate B1", Description = "Kalkış kapısı B1" },
    };

    public MapPage()
    {
        InitializeComponent();
        LocationView.ItemsSource = _locations;
    }

    private void OnLocationSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is LocationItem item)
        {
            DisplayAlert("Yol Tarifi", $"Girişten {item.Name} alanına yönlendirme: Düz ilerleyin, yönlendirme işaretlerini takip edin.", "Tamam");
        }
    }
}

public class LocationItem
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

