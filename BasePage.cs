using Microsoft.AspNetCore.Components;
using Projekt.Models;
using Projekt.Services;

namespace Projekt
{
    public class BasePage : ComponentBase, IDisposable
    {
        [Inject]
        public AuthenticationService Auth { get; set; }

        [Inject]
        public CartService CartService { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            Auth.OnCurrentUserHasChanged += OnCurrentUserChanged;
            CartService.OnCartHasChanged += OnCartChanged;
        }

        private void OnCurrentUserChanged(object? sender, User? user) => InvokeAsync(StateHasChanged);
        private void OnCartChanged(object? sender, List<Product> products) => InvokeAsync(StateHasChanged);

        public void Dispose()
        {
            Auth.OnCurrentUserHasChanged -= OnCurrentUserChanged;
            CartService.OnCartHasChanged -= OnCartChanged;
        }
    }
}
