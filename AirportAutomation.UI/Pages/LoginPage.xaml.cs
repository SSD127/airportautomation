using AirportAutomation.Core.Entities;
using AirportAutomation.Interface;
using AirportAutomation.UI.Helpers;
using AirportAutomation.UI.Models;
using AirportAutomation.UI.Services;

namespace AirportAutomation.UI.Pages;

public partial class LoginPage : ContentPage
{
    private readonly IAuthService _authService;
    private readonly AuthContext _authContext;
    private readonly IGoogleAuthService _googleAuthService;

    private int _tapCount = 0;
    private const int TapThreshold = 5;
    private bool _adminUnlocked = false;

    public LoginPage() : this(
        ServiceHelper.GetRequiredService<IAuthService>(),
        ServiceHelper.GetRequiredService<AuthContext>(),
        ServiceHelper.GetRequiredService<IGoogleAuthService>())
    {
    }

    public LoginPage(IAuthService authService, AuthContext authContext, IGoogleAuthService googleAuthService)
    {
        InitializeComponent();
        _authService = authService;
        _authContext = authContext;
        _googleAuthService = googleAuthService;
    }

    private void OnPassengerLoginClicked(object sender, EventArgs e)
    {
        ErrorLabel.Text = string.Empty;
        var username = PassengerUsernameEntry.Text?.Trim() ?? string.Empty;
        var password = PassengerPasswordEntry.Text?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            ErrorLabel.Text = "Yolcu için kullanıcı adı ve şifre gerekli.";
            return;
        }

        var hata = _authService.Login(username, password, out var user);
        if (!string.IsNullOrEmpty(hata))
        {
            ErrorLabel.Text = hata;
            return;
        }

        if (!IsRole(user, "Passenger"))
        {
            ErrorLabel.Text = "Bu hesap yolcu rolünde değil.";
            return;
        }

        Navigate(user, UserRole.Passenger);
    }

    private void OnStaffLoginClicked(object sender, EventArgs e)
    {
        ErrorLabel.Text = string.Empty;
        var username = StaffUsernameEntry.Text?.Trim() ?? string.Empty;
        var password = StaffPasswordEntry.Text?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            ErrorLabel.Text = "Personel için kullanıcı adı ve şifre gerekli.";
            return;
        }

        var hata = _authService.Login(username, password, out var user);
        if (!string.IsNullOrEmpty(hata))
        {
            ErrorLabel.Text = hata;
            return;
        }

        if (!IsRole(user, "Staff"))
        {
            ErrorLabel.Text = "Bu hesap personel rolünde değil.";
            return;
        }

        Navigate(user, UserRole.Staff);
    }

    private void OnAdminLoginClicked(object sender, EventArgs e)
    {
        ErrorLabel.Text = string.Empty;
        var username = AdminUsernameEntry.Text?.Trim() ?? string.Empty;
        var password = AdminPasswordEntry.Text?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            ErrorLabel.Text = "Admin için kullanıcı adı ve şifre gerekli.";
            return;
        }

        var hata = _authService.Login(username, password, out var user);
        if (!string.IsNullOrEmpty(hata))
        {
            ErrorLabel.Text = hata;
            return;
        }

        if (!IsRole(user, "Admin"))
        {
            ErrorLabel.Text = "Bu hesap admin rolünde değil.";
            return;
        }

        Navigate(user, UserRole.Admin);
    }

    private void OnTitleTapped(object sender, TappedEventArgs e)
    {
        if (_adminUnlocked) return;
        _tapCount++;
        if (_tapCount >= TapThreshold)
        {
            _adminUnlocked = true;
            AdminFrame.IsVisible = true;
            HintLabel.Text = "Admin alanı açıldı.";
        }
        else
        {
            var remaining = TapThreshold - _tapCount;
            HintLabel.Text = $"Admin alanı için {remaining} dokunuş kaldı.";
        }
    }

    private void Navigate(User user, UserRole role)
    {
        _authContext.SetUser(user);
        Application.Current!.MainPage = new AppShell();
    }

    private void OnGoogleLoginClicked(object sender, EventArgs e)
    {
        // Sunum/demoda sahte buton: gerçek Google akışı tetiklenmez
        ErrorLabel.TextColor = Colors.Green;
        ErrorLabel.Text = "Google ile giriş (demo): Yapılandırılmadı, canlı ortamda client ID eklenince açılacak.";
    }

    private void OnPassengerRegisterClicked(object sender, EventArgs e)
    {
        ErrorLabel.Text = string.Empty;
        var username = PassengerUsernameEntry.Text?.Trim() ?? string.Empty;
        var password = PassengerPasswordEntry.Text?.Trim() ?? string.Empty;
        var fullName = username;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            ErrorLabel.Text = "Kayıt için kullanıcı adı ve şifre gerekli.";
            return;
        }

        var hata = _authService.RegisterPassenger(username, password, fullName, out var newUser);
        if (!string.IsNullOrEmpty(hata))
        {
            ErrorLabel.Text = hata;
            return;
        }

        ErrorLabel.TextColor = Colors.Green;
        ErrorLabel.Text = "Kayıt başarılı, giriş yapabilirsiniz.";
    }

    private static bool IsRole(User user, string role) =>
        string.Equals(user.Role, role, StringComparison.OrdinalIgnoreCase);
}

