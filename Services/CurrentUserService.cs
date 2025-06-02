using Projekt.Models;

namespace Projekt.Services
{
    public class CurrentUserService
    {
        public Users? CurrentUser { get; private set; }
        public bool IsAuthenticated => CurrentUser != null;

        public void SignIn(Users user) => CurrentUser = user;
        public void SignOut() => CurrentUser = null;
    }
}
