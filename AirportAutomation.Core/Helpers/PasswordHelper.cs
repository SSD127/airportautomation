using System.Security.Cryptography;
using System.Text;

namespace AirportAutomation.Core.Helpers
{
    public static class PasswordHelper
    {
        public static string CreateSalt(int size = 16)
        {
            var bytes = new byte[size];
            Random.Shared.NextBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        public static string HashPassword(string password, string salt, int iterations = 100_000, int keySize = 32)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(password, Convert.FromBase64String(salt), iterations, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(keySize);
            return Convert.ToBase64String(hash);
        }

        public static bool Verify(string password, string salt, string expectedHash, int iterations = 100_000, int keySize = 32)
        {
            var computed = HashPassword(password, salt, iterations, keySize);
            return CryptographicOperations.FixedTimeEquals(
                Convert.FromBase64String(computed),
                Convert.FromBase64String(expectedHash));
        }
    }
}

