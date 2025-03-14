using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Top2000_MVC.Models;

namespace Top2000_MVC.Controllers
{
    public class RegisterController : Controller
    {
        private readonly HttpClient _httpClient;

        public RegisterController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var contentString = await content.ReadAsStringAsync();

            var response = await _httpClient.PostAsync("register", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Registratie gelukt! Je kunt nu inloggen.";
                return RedirectToAction("Login", "Login"); // ✅ Stuur door naar loginpagina
            }
            else
            {
                TempData["ErrorMessage"] = "Registratie mislukt. Probeer het opnieuw.";
                return RedirectToAction("Login", "Login");
            }
        }
    }
}
