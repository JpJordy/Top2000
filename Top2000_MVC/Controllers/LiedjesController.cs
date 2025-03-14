using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Top2000_MVC.Models;

public class LiedjesController : Controller
{
    private readonly HttpClient _httpClient;

    public LiedjesController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IActionResult> Index(int page = 1)
    {
        var apiUrl = $"https://localhost:7020/api/songs/all?page={page}&pageSize=9";

        var response = await _httpClient.GetAsync(apiUrl);

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Songs = new List<Top2000Song>();
            ViewBag.TotalPages = 1;
            ViewBag.CurrentPage = 1;
            return View();
        }

        var json = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonConvert.DeserializeObject<SongApiResponse>(json);

        if (apiResponse == null || apiResponse.Songs.Count == 0)
        {
            ViewBag.Songs = new List<Top2000Song>();
            ViewBag.TotalPages = 1;
            ViewBag.CurrentPage = 1;
            return View();
        }

        ViewBag.Songs = apiResponse.Songs;
        ViewBag.TotalPages = apiResponse.TotalPages;
        ViewBag.CurrentPage = page;

        return View();
    }
}
