using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Top2000_MVC.Models;

public class ArtistApiResponse
{
    public List<Top2000Artist> Artists { get; set; }
    public int TotalPages { get; set; }
}

public class ArtiestenController : Controller
{
    private readonly HttpClient _httpClient;

    public ArtiestenController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IActionResult> Index(int page = 1)
    {
        var apiUrl = $"https://localhost:7020/api/artists/all?page={page}&pageSize=9";

        var response = await _httpClient.GetAsync(apiUrl);

        if (!response.IsSuccessStatusCode)
        {
            ViewBag.Artists = new List<Top2000Artist>();
            ViewBag.TotalPages = 1;
            return View();
        }

        var json = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonConvert.DeserializeObject<ArtistApiResponse>(json);

        if (apiResponse == null || apiResponse.Artists.Count == 0)
        {
            ViewBag.Artists = new List<Top2000Artist>();
            ViewBag.TotalPages = 1;
            return View();
        }

        ViewBag.Artists = apiResponse.Artists;
        ViewBag.TotalPages = apiResponse.TotalPages;

        return View();
    }
}
