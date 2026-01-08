using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using AirportAutomation.UI.Helpers;
using AirportAutomation.UI.Models;
using Microsoft.Maui.Authentication;

namespace AirportAutomation.UI.Services
{
    public class GoogleAuthService : IGoogleAuthService
    {
        // TODO: Bu değerleri Google Cloud Console'dan alınan native client ID ve redirect URI ile doldurun
        private const string RedirectUri = "com.companyname.airportautomation.ui:/oauth2redirect";
        private const string ClientIdAndroid = "YOUR_ANDROID_CLIENT_ID.apps.googleusercontent.com";
        private const string ClientIdIos = "YOUR_IOS_CLIENT_ID.apps.googleusercontent.com";

        private const string AuthEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
        private const string TokenEndpoint = "https://oauth2.googleapis.com/token";
        private const string Scope = "openid email profile";

        public async Task<(string hata, GoogleAuthResult? result)> LoginAsync()
        {
            try
            {
                var (verifier, challenge) = CreatePkce();
                var clientId = DeviceInfo.Current.Platform == DevicePlatform.iOS ? ClientIdIos : ClientIdAndroid;

                if (clientId.StartsWith("YOUR_", StringComparison.OrdinalIgnoreCase))
                {
                    return ("Google client ID yapılandırılmamış.", null);
                }

                var authUrl = BuildAuthUrl(clientId, challenge);
                var callback = new Uri(RedirectUri);
                var authResult = await WebAuthenticator.Default.AuthenticateAsync(new Uri(authUrl), callback);

                if (!authResult.Properties.TryGetValue("code", out var code) || string.IsNullOrWhiteSpace(code))
                {
                    return ("Google yetkilendirme kodu alınamadı.", null);
                }

                var token = await ExchangeCodeForTokenAsync(clientId, verifier, code);
                if (token == null) return ("Google token alınamadı.", null);

                var idToken = token.RootElement.GetProperty("id_token").GetString() ?? string.Empty;
                var profile = ParseIdToken(idToken);
                if (string.IsNullOrWhiteSpace(profile.email))
                {
                    return ("Google profilinde e-posta bulunamadı.", null);
                }

                return (string.Empty, new GoogleAuthResult
                {
                    Email = profile.email,
                    Name = profile.name,
                    IdToken = idToken
                });
            }
            catch (TaskCanceledException)
            {
                return ("Google oturumu iptal edildi.", null);
            }
            catch (Exception ex)
            {
                return (ex.Message, null);
            }
        }

        private static string BuildAuthUrl(string clientId, string challenge)
        {
            var sb = new StringBuilder();
            sb.Append(AuthEndpoint);
            sb.Append("?response_type=code");
            sb.Append("&client_id=").Append(Uri.EscapeDataString(clientId));
            sb.Append("&redirect_uri=").Append(Uri.EscapeDataString(RedirectUri));
            sb.Append("&scope=").Append(Uri.EscapeDataString(Scope));
            sb.Append("&code_challenge=").Append(Uri.EscapeDataString(challenge));
            sb.Append("&code_challenge_method=S256");
            sb.Append("&prompt=select_account");
            return sb.ToString();
        }

        private static async Task<JsonDocument?> ExchangeCodeForTokenAsync(string clientId, string verifier, string code)
        {
            var payload = new Dictionary<string, string>
            {
                ["code"] = code,
                ["client_id"] = clientId,
                ["redirect_uri"] = RedirectUri,
                ["grant_type"] = "authorization_code",
                ["code_verifier"] = verifier
            };

            using var client = new HttpClient();
            var res = await client.PostAsync(TokenEndpoint, new FormUrlEncodedContent(payload));
            if (!res.IsSuccessStatusCode) return null;
            var stream = await res.Content.ReadAsStreamAsync();
            return await JsonDocument.ParseAsync(stream);
        }

        private static (string verifier, string challenge) CreatePkce()
        {
            var bytes = new byte[32];
            RandomNumberGenerator.Fill(bytes);
            var verifier = Base64Url(Convert.ToBase64String(bytes));
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(verifier));
            var challenge = Base64Url(Convert.ToBase64String(hash));
            return (verifier, challenge);
        }

        private static string Base64Url(string value) =>
            value.Replace("+", "-").Replace("/", "_").Replace("=", "");

        private static (string email, string name) ParseIdToken(string idToken)
        {
            try
            {
                var parts = idToken.Split('.');
                if (parts.Length < 2) return (string.Empty, string.Empty);
                var payload = parts[1];
                var padded = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');
                var bytes = Convert.FromBase64String(padded.Replace("-", "+").Replace("_", "/"));
                var json = JsonDocument.Parse(bytes);
                var root = json.RootElement;
                var email = root.TryGetProperty("email", out var e) ? e.GetString() ?? string.Empty : string.Empty;
                var name = root.TryGetProperty("name", out var n) ? n.GetString() ?? string.Empty : string.Empty;
                return (email, name);
            }
            catch
            {
                return (string.Empty, string.Empty);
            }
        }
    }
}

