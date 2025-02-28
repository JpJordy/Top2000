using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly HttpClient _httpClient;

    public AdminController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("ApiClient");
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUser(string username)
    {
        var response = await _httpClient.DeleteAsync($"admin/deleteUser/{username}");

        if (response.IsSuccessStatusCode)
        {
            TempData["SuccessMessage"] = $"Gebruiker {username} succesvol verwijderd!";
        }
        else
        {
            TempData["ErrorMessage"] = "Fout bij verwijderen gebruiker.";
        }

        return RedirectToAction("Index");
    }
}
