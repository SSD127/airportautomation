namespace AirportAutomation.UI;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		// Açılışta giriş sayfası
		MainPage = new Pages.LoginPage();
	}
}
