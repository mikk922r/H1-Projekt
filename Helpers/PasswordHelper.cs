using System.Security.Cryptography;

namespace Projekt.Helpers
{
    public static class PasswordHelper
    {
        private const int SaltSize = 16; // 128 bit
        private const int KeySize = 32; // 256 bit
        private const int Iterations = 10000;
        private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;
        private const char Delimiter = ':';

        public static string Hash(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, KeySize);

            string hashBase64 = Convert.ToBase64String(hash);
            string saltBase64 = Convert.ToBase64String(salt);

            return string.Join(Delimiter, hashBase64, saltBase64, Iterations, Algorithm);
        }

        public static bool Verify(string passwordHash, string inputPassword)
        {
            var elements = passwordHash.Split(Delimiter);

            var hash = Convert.FromBase64String(elements[0]);
            var salt = Convert.FromBase64String(elements[1]);

            var iterations = int.Parse(elements[2]);
            var algorithm = new HashAlgorithmName(elements[3]);

            var inputHash = Rfc2898DeriveBytes.Pbkdf2(inputPassword, salt, iterations, algorithm, hash.Length);

            return CryptographicOperations.FixedTimeEquals(hash, inputHash);
        }
    }
}
