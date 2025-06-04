using Microsoft.AspNetCore.Identity;
using Projekt.Models;

namespace Projekt.Helpers
{
    public static class PasswordHelper
    {
        private static readonly PasswordHasher<User> passwordHasher = new PasswordHasher<User>();

        private static readonly User fakeUser = new User();

        public static string Hash(string password)
        {
            return passwordHasher.HashPassword(fakeUser, password);
        }

        public static bool Verify(string passwordHash, string inputPassword)
        {
            return passwordHasher.VerifyHashedPassword(fakeUser, passwordHash, inputPassword) is PasswordVerificationResult.Success;
        }
    }
}
