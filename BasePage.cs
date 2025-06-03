using Microsoft.AspNetCore.Components;
using Projekt.Models;
using Projekt.Services;

namespace Projekt
{
    public class BasePage : ComponentBase, IDisposable
    {
        [Inject]
        public AuthenticationService Auth { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            Auth.OnCurrentUserHasChanged += OnCurrentUserChanged;
        }

        private void OnCurrentUserChanged(object? sender, User? user)
        {
            InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            Auth.OnCurrentUserHasChanged -= OnCurrentUserChanged;
        }
    }
}
