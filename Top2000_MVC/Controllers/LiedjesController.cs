using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Top2000_MVC.Models;


public class SongApiResponse
{
    public List<Top2000Song> Songs { get; set; }
    public int TotalPages { get; set; }
}


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
            Console.WriteLine("API Error: " + response.StatusCode);
            ViewBag.Songs = new List<Top2000Song>();
            ViewBag.TotalPages = 1;
            ViewBag.CurrentPage = 1;
            return View("~/Views/Liedjes/Index.cshtml");
        }

        var json = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonConvert.DeserializeObject<SongApiResponse>(json); 

        if (apiResponse == null || apiResponse.Songs.Count == 0)
        {
            Console.WriteLine("API response is null or does not contain songs.");
            ViewBag.Songs = new List<Top2000Song>();
            ViewBag.TotalPages = 1;
            ViewBag.CurrentPage = 1;
            return View("~/Views/Liedjes/Index.cshtml");
        }

        List<Top2000Song> songs = apiResponse.Songs.Select(song => new Top2000Song
        {
            SongId = song.SongId,
            Titel = song.Titel,
            Jaar = song.Jaar,
            Afbeelding = song.Afbeelding,
            Lyrics = song.Lyrics,
            Youtube = song.Youtube,
            ArtiestId = song.ArtiestId,
            DurationMs = song.DurationMs,
            Popularity = song.Popularity,
            SpotifyUrls = song.SpotifyUrls
        }).ToList();

        ViewBag.Songs = songs;
        ViewBag.TotalPages = apiResponse.TotalPages;
        ViewBag.CurrentPage = page;

        return View("~/Views/Liedjes/Index.cshtml");
    }
}
