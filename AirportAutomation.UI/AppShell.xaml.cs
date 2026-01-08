using AirportAutomation.UI.Helpers;
using AirportAutomation.UI.Services;

namespace AirportAutomation.UI;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		ApplyRoleBasedTabs();
	}

	private void ApplyRoleBasedTabs()
	{
		var auth = ServiceHelper.GetRequiredService<AuthContext>();
		var user = auth.CurrentUser;

		// Admin olmayanlar için admin sekmesini kaldır
		if (user == null || !auth.IsInRole("Admin"))
		{
			MainTabs.Items.Remove(AdminTab);
		}

		// Personel/admin olmayanlar için kapı yönetimi sekmesini kaldır
		if (user == null || !auth.IsStaffOrAdmin())
		{
			MainTabs.Items.Remove(GateTab);
		}
	}
}
