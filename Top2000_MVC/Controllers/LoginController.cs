using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Top2000_MVC.Models;

namespace Top2000_MVC.Controllers
{
    public class LoginController : Controller
    {
        private readonly HttpClient _httpClient;

        public LoginController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        [HttpGet] // ✅ Hiermee wordt de loginpagina correct geladen
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost] // ✅ Verstuurt login-informatie naar de API
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("login", content);

            if (response.IsSuccessStatusCode)
            {
                var userJson = await response.Content.ReadAsStringAsync();
                var user = JsonSerializer.Deserialize<UserViewModel>(userJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role) // Voeg rol toe aan claims
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties { IsPersistent = true };

                // Log in de gebruiker en stel een cookie in
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                // Debugging: Controleer of de cookie is ingesteld
                Console.WriteLine("✅ Cookie zou nu aangemaakt moeten zijn!");
                Console.WriteLine($"🔍 Ingelogde gebruiker: {user.Username}");

                return RedirectToAction("Index", "Home"); // Redirect naar Home na succesvolle login
            }

            ModelState.AddModelError("", "Ongeldige inloggegevens.");
            return View(model);
        }

        [HttpGet] // ✅ Uitlogfunctionaliteit
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Login");
        }
    }
}
