using Projekt.Components;
using Projekt.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Projekt
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Server–style components:
            builder.Services
                .AddRazorComponents()
                .AddInteractiveServerComponents();

            // Scoped services:
            builder.Services.AddScoped<DBService>();
            builder.Services.AddScoped<AuthenticationService>();
            builder.Services.AddScoped<OrderService>();
            builder.Services.AddScoped<CartService>();

            // Cookie baseret auth:
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/login";
                    options.LogoutPath = "/logout";
                });

            var app = builder.Build();

            // 5) Middleware pipeline in exact order:
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            // Authentication + Authorization come before antiforgery
            app.UseAuthentication();
            app.UseAuthorization();

            // Now register the antiforgery middleware
            app.UseAntiforgery();

         
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}