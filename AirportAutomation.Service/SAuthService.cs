using AirportAutomation.Core.Data;
using AirportAutomation.Core.Entities;
using AirportAutomation.Core.Helpers;
using AirportAutomation.Interface;

namespace AirportAutomation.Service
{
    public class SAuthService : IAuthService
    {
        public string RegisterPassenger(string username, string password, string fullName, out User user)
        {
            user = new User();
            string? hata = null;

            try
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    return "Kullanıcı adı ve şifre gerekli.";
                }

                using var context = new AirportDbContext();
                if (context.Users.Any(u => u.Username == username))
                {
                    return "Kullanıcı adı zaten mevcut.";
                }

                var salt = PasswordHelper.CreateSalt();
                var hash = PasswordHelper.HashPassword(password, salt);

                user = new User
                {
                    Username = username,
                    FullName = fullName ?? string.Empty,
                    Role = "Passenger",
                    Salt = salt,
                    PasswordHash = hash
                };

                context.Users.Add(user);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                hata = ex.Message;
            }

            return hata ?? string.Empty;
        }

        public string EnsureExternalPassenger(string username, string fullName, out User user)
        {
            user = new User();
            string? hata = null;

            try
            {
                using var context = new AirportDbContext();
                var existing = context.Users.FirstOrDefault(u => u.Username == username);
                if (existing != null)
                {
                    user = existing;
                    return string.Empty;
                }

                var salt = PasswordHelper.CreateSalt();
                var randomPass = PasswordHelper.CreateSalt(24); // random pwd
                var hash = PasswordHelper.HashPassword(randomPass, salt);

                user = new User
                {
                    Username = username,
                    FullName = fullName ?? string.Empty,
                    Role = "Passenger",
                    Salt = salt,
                    PasswordHash = hash
                };

                context.Users.Add(user);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                hata = ex.Message;
            }

            return hata ?? string.Empty;
        }

        public string Login(string username, string password, out User user)
        {
            user = new User();
            string? hata = null;

            try
            {
                using var context = new AirportDbContext();
                var dbUser = context.Users.FirstOrDefault(u => u.Username == username);
                if (dbUser == null)
                {
                    return "Kullanıcı bulunamadı.";
                }

                if (!PasswordHelper.Verify(password, dbUser.Salt, dbUser.PasswordHash))
                {
                    return "Şifre hatalı.";
                }

                user = dbUser;
            }
            catch (Exception ex)
            {
                hata = ex.Message;
            }

            return hata ?? string.Empty;
        }
    }
}

