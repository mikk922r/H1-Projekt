using Projekt.Helpers;
using Projekt.Models;
using Projekt.Models.Forms;

namespace Projekt.Services
{
    public class AuthenticationService
    {
        private readonly DBService _dBService;

        public AuthenticationService(DBService dBService)
        {
            _dBService = dBService;
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

            return user;
        }

        public async Task<User?> Register(RegisterForm registerForm)
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

        public void SignOut() => SetCurrentUser(null);

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
