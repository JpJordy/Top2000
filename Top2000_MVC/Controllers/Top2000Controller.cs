using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
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
            var apiUrl = $"https://localhost:7020/api/songs?page={page}&pageSize=6";
            var response = await _httpClient.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("API Error: " + response.StatusCode);
                ViewBag.Songs = new List<Top2000Song>();
                ViewBag.TotalPages = 1;
                return View();
            }

            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<dynamic>(json);

            if (apiResponse == null || apiResponse.songs == null)
            {
                Console.WriteLine("API response is null or does not contain 'songs'.");
                Console.WriteLine($"Response Content: {json}"); 
                ViewBag.Songs = new List<Top2000Song>();
                ViewBag.TotalPages = 1; 
                return View();
            }

            var songs = apiResponse.songs.ToObject<List<Top2000Song>>(); 
            var totalPages = (int)apiResponse.totalPages;

            ViewBag.Songs = songs;
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;

            return View();
        }
    }
}
