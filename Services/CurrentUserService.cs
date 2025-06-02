using Projekt.Models;

namespace Projekt.Services
{
    public class CurrentUserService
    {
        public event EventHandler<User?> CurrentUserHasChanged;

        public User? CurrentUser { get; private set; }
        public bool IsAuthenticated => CurrentUser != null;

        public void SignIn(User user) => SetCurrentUser(user);
        public void SignOut() => SetCurrentUser(null);

        protected virtual void OnCurrentUserChanged(User? e)
        {
            EventHandler<User?> handler = CurrentUserHasChanged;

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
