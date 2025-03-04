using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Top2000_MVC.ViewModels;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly HttpClient _httpClient;

    public AdminController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("ApiClient");
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var response = await _httpClient.GetAsync("admin/getUsers");

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Kon gebruikerslijst niet ophalen.";
                return View(new List<AdminUserWithRole>());
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            Console.WriteLine("API Response JSON: " + jsonString);

            var userList = JsonSerializer.Deserialize<AdminUserList>(jsonString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (userList == null || userList.Values == null || !userList.Values.Any())
            {
                TempData["ErrorMessage"] = "Geen gebruikers gevonden.";
                return View(new List<AdminUserWithRole>());
            }

            var users = userList.Values;
            ViewBag.Users = users;
            return View(users);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Fout bij ophalen gebruikers: " + ex.Message);
            TempData["ErrorMessage"] = "Er is een fout opgetreden bij het ophalen van de gebruikers.";
            return View(new List<AdminUserWithRole>());
        }
    }

    [HttpPost]
    public async Task<IActionResult> UpdateRole(string username)
    {
        var response = await _httpClient.PostAsync($"admin/updateRole/{username}/Admin", null);

        if (response.IsSuccessStatusCode)
        {
            TempData["SuccessMessage"] = $"Rol van gebruiker {username} succesvol gewijzigd naar Admin!";
        }
        else
        {
            TempData["ErrorMessage"] = "Fout bij het wijzigen van de rol.";
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> RemoveAdmin(string username)
    {
        var response = await _httpClient.PostAsync($"admin/removeAdmin/{username}", null);

        if (response.IsSuccessStatusCode)
        {
            TempData["SuccessMessage"] = $"Rol van gebruiker {username} succesvol gewijzigd naar User!";
        }
        else
        {
            TempData["ErrorMessage"] = "Fout bij het verwijderen van admin.";
        }

        return RedirectToAction("Index");
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

    public async Task<IActionResult> EditSong(int id)
    {
        var response = await _httpClient.GetAsync($"songs/{id}");

        if (!response.IsSuccessStatusCode)
        {
            TempData["ErrorMessage"] = "Kon nummer niet ophalen.";
            return RedirectToAction("Index");
        }

        var jsonString = await response.Content.ReadAsStringAsync();
        var song = JsonSerializer.Deserialize<SongViewModel>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return View(song);
    }




    [HttpPost]
    public async Task<IActionResult> EditSong(SongViewModel song)
    {
        var updateDto = new
        {
            Lyrics = song.Lyrics,
            Afbeelding = song.Afbeelding,
            Youtube = song.Youtube
        };

        var jsonContent = new StringContent(JsonSerializer.Serialize(updateDto), System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync($"admin/updateSong/{song.SongId}", jsonContent);

        if (response.IsSuccessStatusCode)
        {
            TempData["SuccessMessage"] = "Nummergegevens succesvol bijgewerkt!";
            return RedirectToAction("Index");
        }
        else
        {
            TempData["ErrorMessage"] = "Fout bij het bijwerken van het nummer.";
            return View(song);
        }
    }




    public async Task<IActionResult> EditArtiest(int id)
    {
        var response = await _httpClient.GetAsync($"artiesten/{id}");

        if (!response.IsSuccessStatusCode)
        {
            TempData["ErrorMessage"] = "Kon artiest niet ophalen.";
            return RedirectToAction("Index");
        }

        var jsonString = await response.Content.ReadAsStringAsync();
        var artiest = JsonSerializer.Deserialize<ArtiestViewModel>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (artiest == null)
        {
            TempData["ErrorMessage"] = "Artiest niet gevonden.";
            return RedirectToAction("Index");
        }

        return View(artiest);
    }




    [HttpPost]
    public async Task<IActionResult> EditArtiest(ArtiestViewModel artiest)
    {
        var updateDto = new
        {
            wiki = artiest.Wiki,
            biografie = artiest.Biografie,
            foto = artiest.Foto
        };

        var jsonContent = new StringContent(JsonSerializer.Serialize(updateDto), System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync($"admin/updateArtiest/{artiest.ArtiestId}", jsonContent);

        if (response.IsSuccessStatusCode)
        {
            TempData["SuccessMessage"] = "Nummergegevens succesvol bijgewerkt!";
            return RedirectToAction("Index");
        }
        else
        {
            TempData["ErrorMessage"] = "Fout bij het bijwerken van het nummer.";
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

