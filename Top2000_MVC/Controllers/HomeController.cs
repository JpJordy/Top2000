using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Top2000_MVC.Models;

namespace Top2000_MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;

        public HomeController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<IActionResult> Index()
        {
            var apiUrl = $"https://localhost:7020/api/songs?page=1&pageSize=5";

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

            List<Top2000Song> songs = apiResponse.songs.ToObject<List<Top2000Song>>();
            ViewBag.Songs = songs;
            return View();
        }
    }
}
