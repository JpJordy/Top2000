using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using Top2000_MVC.Models;
using Top2000_MVC.ViewModels;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly HttpClient _httpClient;
    private readonly ApiSettings _apiSettings;

    public AdminController(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings)
    {
        _httpClient = httpClientFactory.CreateClient("ApiClient");
        _apiSettings = apiSettings.Value;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_apiSettings.BaseUrl}/admin/getUsers");

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Kon gebruikerslijst niet ophalen.";
                return View(new List<AdminUserWithRole>());
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            var userList = JsonSerializer.Deserialize<AdminUserList>(jsonString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var users = userList?.Values ?? new List<AdminUserWithRole>();
            return View(users);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fout bij ophalen gebruikers: {ex.Message}");
            TempData["ErrorMessage"] = "Er is een fout opgetreden bij het ophalen van de gebruikers.";
            return View(new List<AdminUserWithRole>());
        }
    }

    [HttpGet]
    public async Task<IActionResult> EditSong(string title)
    {
        var response = await _httpClient.GetAsync($"{_apiSettings.BaseUrl}/Admin/getSong/{title}");

        if (!response.IsSuccessStatusCode)
        {
            TempData["ErrorMessage"] = "Nummer niet gevonden.";
            return RedirectToAction("Index");
        }

        var song = await response.Content.ReadFromJsonAsync<SongViewModel>();
        return View(song);
    }

    [HttpGet]
    public async Task<IActionResult> EditArtiest(string naam)
    {
        var response = await _httpClient.GetAsync($"{_apiSettings.BaseUrl}/Admin/getArtiest/{naam}");

        if (!response.IsSuccessStatusCode)
        {
            TempData["ErrorMessage"] = "Artiest niet gevonden.";
            return RedirectToAction("Index");
        }

        var artiest = await response.Content.ReadFromJsonAsync<ArtiestViewModel>();
        return View(artiest);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateRole(string username)
    {
        var response = await _httpClient.PostAsync($"{_apiSettings.BaseUrl}/admin/updateRole/{username}/Admin", null);

        TempData[response.IsSuccessStatusCode ? "SuccessMessage" : "ErrorMessage"] =
            response.IsSuccessStatusCode ? $"Rol van gebruiker {username} succesvol gewijzigd naar Admin!" :
            "Fout bij het wijzigen van de rol.";

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> RemoveAdmin(string username)
    {
        var response = await _httpClient.PostAsync($"{_apiSettings.BaseUrl}/admin/removeAdmin/{username}", null);

        TempData[response.IsSuccessStatusCode ? "SuccessMessage" : "ErrorMessage"] =
            response.IsSuccessStatusCode ? $"Rol van gebruiker {username} succesvol gewijzigd naar User!" :
            "Fout bij het verwijderen van admin.";

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUser(string username)
    {
        var response = await _httpClient.DeleteAsync($"{_apiSettings.BaseUrl}/admin/deleteUser/{username}");

        TempData[response.IsSuccessStatusCode ? "SuccessMessage" : "ErrorMessage"] =
            response.IsSuccessStatusCode ? $"Gebruiker {username} succesvol verwijderd!" :
            "Fout bij verwijderen gebruiker.";

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> EditSong(SongViewModel song)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Ongeldige gegevens.";
            return View(song);
        }

        var updateDto = new
        {
            song.Afbeelding,
            song.Lyrics,
            song.Youtube
        };

        var jsonContent = new StringContent(JsonSerializer.Serialize(updateDto), Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync($"{_apiSettings.BaseUrl}/Admin/updateSong/{song.Titel}", jsonContent);

        if (response.IsSuccessStatusCode)
        {
            TempData["SuccessMessage"] = "Nummergegevens succesvol bijgewerkt!";
            return RedirectToAction("Index");
        }
        else
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            TempData["ErrorMessage"] = string.IsNullOrEmpty(errorMessage) ? "Fout bij het bijwerken van het nummer." : errorMessage;
            return View(song);
        }
    }

    [HttpPost]
    public async Task<IActionResult> EditArtiest(ArtiestViewModel artiest)
    {
        var updateDto = new
        {
            artiest.Wiki,
            artiest.Biografie,
            artiest.Foto
        };

        var jsonContent = new StringContent(JsonSerializer.Serialize(updateDto), Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync($"{_apiSettings.BaseUrl}/Admin/updateArtiest/{artiest.Naam}", jsonContent);

        if (response.IsSuccessStatusCode)
        {
            TempData["SuccessMessage"] = "Artiestgegevens succesvol bijgewerkt!";
            return RedirectToAction("Index");
        }
        else
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            TempData["ErrorMessage"] = string.IsNullOrEmpty(errorMessage) ? "Fout bij het bijwerken van artiestgegevens." : errorMessage;
            return View(artiest);
        }
    }
}