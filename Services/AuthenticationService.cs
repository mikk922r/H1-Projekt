using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Projekt.Helpers;
using Projekt.Models;
using Projekt.Models.Forms;
using System.Threading.Tasks;

namespace Projekt.Services
{
    public class AuthenticationService
    {
        private readonly DBService _dBService;
        private readonly ProtectedSessionStorage _protectedSessionStorage;

        public AuthenticationService(DBService dBService, ProtectedSessionStorage protectedSessionStorage)
        {
            _dBService = dBService;
            _protectedSessionStorage = protectedSessionStorage;
        }

        public event EventHandler<User?> OnCurrentUserHasChanged;

        public User? CurrentUser { get; private set; }

        public bool IsAuthenticated => CurrentUser != null;

        public async Task<User?> SignIn(string email, string password)
        {
            User? user = await _dBService.GetUserByEmailAsync(email);

            if (user is null)
            {
                return null;
            }

            if (!PasswordHelper.Verify(user.Password, password))
            {
                return null;
            }

            SetCurrentUser(user);

            await _protectedSessionStorage.SetAsync("currentUser", user);

            return user;
        }

        public async Task<User?> Register(RegisterFormModel registerForm)
        {
            User user = new User()
            {
                Name = registerForm.Name,
                Email = registerForm.Email,
                PhoneCode = registerForm.PhoneCode,
                PhoneNumber = registerForm.PhoneNumber,
                Password = PasswordHelper.Hash(registerForm.Password)
            };

            int? id = await _dBService.AddUserAsync(user);

            if (!id.HasValue)
            {
                return null;
            }

            user.Id = id.Value;

            return user;
        }

        public async Task SignOut()
        {
            SetCurrentUser(null);

            await _protectedSessionStorage.DeleteAsync("currentUser");
        }

        public async Task CheckSessionStorage()
        {
            ProtectedBrowserStorageResult<User> user = await _protectedSessionStorage.GetAsync<User>("currentUser");

            if (!user.Success)
            {
                return;
            }

            if (user.Value is null)
            {
                return;
            }

            SetCurrentUser(user.Value);
        }

        protected virtual void OnCurrentUserChanged(User? e)
        {
            EventHandler<User?> handler = OnCurrentUserHasChanged;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void SetCurrentUser(User? user)
        {
            CurrentUser = user;

            OnCurrentUserChanged(user);
        }
    }
}
