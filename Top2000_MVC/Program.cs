using Microsoft.AspNetCore.Authentication.Cookies;

namespace Top2000_MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Voeg authenticatie toe (Cookies)
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Login/Login"; // Verwijst naar LoginController
                    options.AccessDeniedPath = "/Login/AccessDenied";
                });

            // Voeg autorisatie toe
            builder.Services.AddAuthorization();

            // Voeg controllers en views toe
            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpClient();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // **Gebruik Authenticatie en Autorisatie**
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
