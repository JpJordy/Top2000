using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Differencing;
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


public class ArtistsController : Controller
{
    private readonly HttpClient _httpClient;

    public ArtistsController(HttpClient httpClient)
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
            ViewBag.CurrentPage = 1;
            return View("~/Views/Artiesten/Index.cshtml");
        }

        var json = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonConvert.DeserializeObject<ArtistApiResponse>(json);

        if (apiResponse == null || apiResponse.Artists.Count == 0)
        {

            ViewBag.Artists = new List<Top2000Artist>();
            ViewBag.TotalPages = 1;
            ViewBag.CurrentPage = 1;
            return View("~/Views/Artiesten/Index.cshtml");
        }

        List<Top2000Artist> artists = apiResponse.Artists.Select(artist => new Top2000Artist
        {
            ArtiestId = artist.ArtiestId,
            Naam = artist.Naam,
            Wiki = artist.Wiki,
            Genres = artist.Genres,
            Foto = artist.Foto
        }).ToList();

        ViewBag.Artists = artists;
        ViewBag.TotalPages = apiResponse.TotalPages;
        ViewBag.CurrentPage = page;

        return View("~/Views/Artiesten/Index.cshtml");
    }
}
