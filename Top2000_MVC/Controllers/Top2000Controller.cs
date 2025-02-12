using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Top2000_MVC.Models;

namespace Top2000_MVC.Controllers
{
    public class Top2000Controller : Controller
    {
        private readonly HttpClient _httpClient;

        public Top2000Controller(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            if (page > 334)
            {
                page = 334;
            }
            var apiUrl = $"https://localhost:7020/api/songs?page={page}&pageSize=6";
            var response = await _httpClient.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("API Error: " + response.StatusCode);
                ViewBag.Songs = new List<Top2000Song>();
                ViewBag.TotalPages = 1;
                ViewBag.CurrentPage = 1;
                return View();
            }

            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<dynamic>(json);

            if (apiResponse == null || apiResponse.songs == null)
            {
                Console.WriteLine("API response is null or does not contain 'songs'.");
                ViewBag.Songs = new List<Top2000Song>();
                ViewBag.TotalPages = 1;
                ViewBag.CurrentPage = 1;
                return View();
            }

            var songs = apiResponse.songs.ToObject<List<Top2000Song>>();
            var totalPages = (int)apiResponse.totalPages;

            if (totalPages > 334)
            {
                totalPages = 334;
            }

            if (page > totalPages)
            {
                page = totalPages;
            }

            ViewBag.Songs = songs;
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;

            return View();
        }

        public async Task<IActionResult> ArtistInfo(string artistName, string wiki)
        {
            if (string.IsNullOrEmpty(artistName))
            {
                return RedirectToAction("Index");
            }

            var apiUrl = $"https://localhost:7020/api/songs/artist/{artistName}";
            var response = await _httpClient.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("API Error: " + response.StatusCode);
                ViewBag.Artist = null;
                return View("~/Views/ArtiestInfo/Index.cshtml"); // Explicitly specify the location of the view
            }

            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<Top2000Artist>(json);

            if (apiResponse == null)
            {
                Console.WriteLine("API response is null or does not contain artist data.");
                ViewBag.Artist = null;
                return View("~/Views/ArtiestInfo/Index.cshtml");
            }

            ViewBag.Artist = apiResponse;
            ViewBag.Wiki = wiki;

            Console.WriteLine(ViewBag.wiki);

            return View("~/Views/ArtiestInfo/Index.cshtml"); // Return naar de view
        }
    }
}
