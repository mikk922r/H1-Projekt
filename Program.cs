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

            // 1) Add Blazor Server–style components:
            builder.Services
                .AddRazorComponents()
                .AddInteractiveServerComponents();

            // 2) Your scoped services:
            builder.Services.AddScoped<DBService>();
            builder.Services.AddScoped<AuthenticationService>();

            // 3) Cookie-based auth:
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/login";
                    options.LogoutPath = "/logout";
                });

            // 4) Build the app:
            var app = builder.Build();

            // 5) Middleware pipeline in exact order:
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            // ?? Authentication + Authorization come before antiforgery
            app.UseAuthentication();
            app.UseAuthorization();

            // ?? Now register the antiforgery middleware
            app.UseAntiforgery();

            // 6) Map your Blazor components (no _Host page needed)
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}