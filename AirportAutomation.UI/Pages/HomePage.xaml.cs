namespace AirportAutomation.UI.Pages;

public partial class HomePage : ContentPage
{
	public HomePage()
	{
		InitializeComponent();
	}

    private Task NavigateAsync(string route) => Shell.Current.GoToAsync($"//{route}");

    private void OnFlightsClicked(object sender, EventArgs e) => _ = NavigateAsync("MainPage");
    private void OnGatesClicked(object sender, EventArgs e) => _ = NavigateAsync("GatePage");
    private void OnPassengersClicked(object sender, EventArgs e) => _ = NavigateAsync("PassengerPage");
    private void OnMapClicked(object sender, EventArgs e) => _ = NavigateAsync("MapPage");
    private void OnAssistantClicked(object sender, EventArgs e) => _ = NavigateAsync("AssistantPage");
}

