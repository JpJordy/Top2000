using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Top2000_MVC.ViewModels;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly HttpClient _httpClient;
    private const string ApiBaseUrl = "https://localhost:7020/api"; // Zet hier de juiste API URL

    public AdminController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("ApiClient");
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{ApiBaseUrl}/admin/getUsers");

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
            TempData["ErrorMessage"] = "Er is een fout opgetreden bij het ophalen van de gebruikers.";
            return View(new List<AdminUserWithRole>());
        }
    }

    [HttpPost]
    public async Task<IActionResult> UpdateRole(string username)
    {
        var response = await _httpClient.PostAsync($"{ApiBaseUrl}/admin/updateRole/{username}/Admin", null);

        TempData[response.IsSuccessStatusCode ? "SuccessMessage" : "ErrorMessage"] =
            response.IsSuccessStatusCode ? $"Rol van gebruiker {username} succesvol gewijzigd naar Admin!" :
            "Fout bij het wijzigen van de rol.";

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> RemoveAdmin(string username)
    {
        var response = await _httpClient.PostAsync($"{ApiBaseUrl}/admin/removeAdmin/{username}", null);

        TempData[response.IsSuccessStatusCode ? "SuccessMessage" : "ErrorMessage"] =
            response.IsSuccessStatusCode ? $"Rol van gebruiker {username} succesvol gewijzigd naar User!" :
            "Fout bij het verwijderen van admin.";

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUser(string username)
    {
        var response = await _httpClient.DeleteAsync($"{ApiBaseUrl}/admin/deleteUser/{username}");

        TempData[response.IsSuccessStatusCode ? "SuccessMessage" : "ErrorMessage"] =
            response.IsSuccessStatusCode ? $"Gebruiker {username} succesvol verwijderd!" :
            "Fout bij verwijderen gebruiker.";

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> EditSong(string title)
    {
        var response = await _httpClient.GetAsync($"{ApiBaseUrl}/Admin/getSong/{title}");

        if (!response.IsSuccessStatusCode)
        {
            TempData["ErrorMessage"] = "Nummer niet gevonden.";
            return RedirectToAction("Index");
        }

        var song = await response.Content.ReadFromJsonAsync<SongViewModel>();
        return View(song);
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
            Afbeelding = song.Afbeelding,
            Lyrics = song.Lyrics,
            Youtube = song.Youtube
        };

        var jsonContent = new StringContent(JsonSerializer.Serialize(updateDto), Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync($"{ApiBaseUrl}/Admin/updateSong/{song.Titel}", jsonContent);

        TempData[response.IsSuccessStatusCode ? "SuccessMessage" : "ErrorMessage"] =
            response.IsSuccessStatusCode ? "Nummergegevens succesvol bijgewerkt!" :
            "Fout bij het bijwerken van het nummer.";

        return response.IsSuccessStatusCode ? RedirectToAction("Index") : View(song);
    }


    public async Task<IActionResult> EditArtiest(string naam)
    {
        var response = await _httpClient.GetAsync($"{ApiBaseUrl}/Admin/getArtiest/{naam}");

        if (!response.IsSuccessStatusCode)
        {
            TempData["ErrorMessage"] = "Artiest niet gevonden.";
            return RedirectToAction("Index");
        }

        var artiest = await response.Content.ReadFromJsonAsync<ArtiestViewModel>();
        return View(artiest);
    }

    [HttpPost]
    public async Task<IActionResult> EditArtiest(ArtiestViewModel artiest)
    {
        var updateDto = new
        {
            Wiki = artiest.Wiki,
            Biografie = artiest.Biografie,
            Foto = artiest.Foto
        };

        var jsonContent = new StringContent(JsonSerializer.Serialize(updateDto), Encoding.UTF8, "application/json");

        var response = await _httpClient.PutAsync($"{ApiBaseUrl}/Admin/updateArtiest/{artiest.Naam}", jsonContent);

        if (response.IsSuccessStatusCode)
        {
            TempData["SuccessMessage"] = "Artiestgegevens succesvol bijgewerkt!";
            return RedirectToAction("Index");
        }
        else
        {
            TempData["ErrorMessage"] = response;
            return View(artiest);
        }
    }

    public class AdminUserWithRole
    {
        public string UserName { get; set; }
        public string Role { get; set; }
    }

    public class AdminUserList
    {
        [JsonPropertyName("$values")]
        public List<AdminUserWithRole> Values { get; set; }
    }
}
