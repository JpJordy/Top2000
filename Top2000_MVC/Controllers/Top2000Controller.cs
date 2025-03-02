using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public async Task<IActionResult> Index(int page = 1, int? top2000Year = null, string? artist = null, string? sortBy = null, string? searchQuery = null, string? searchType = "title")
        {
            if (page > 334)
            {
                page = 334;
            }

            var apiUrl = $"https://localhost:7020/api/songs?page={page}&pageSize=6";

            if (!string.IsNullOrEmpty(searchQuery))
            {
                if (searchType == "artist")
                {
                    apiUrl += $"&artist={Uri.EscapeDataString(searchQuery)}";
                }
                else
                {
                    apiUrl += $"&searchQuery={Uri.EscapeDataString(searchQuery)}";
                }
            }

            if (!string.IsNullOrEmpty(sortBy))
            {
                apiUrl += $"&sortBy={sortBy}";
            }
            else
            {
                sortBy = "positie";
                apiUrl += $"&sortBy=positie";
            }

            if (top2000Year.HasValue)
            {
                apiUrl += $"&top2000Year={top2000Year.Value}";
            }
            else
            {
                top2000Year = 2023;
                apiUrl += $"&top2000Year=2023";
            }

            ViewBag.SelectedTop2000Year = top2000Year;

            var response = await _httpClient.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("API Error: " + response.StatusCode);
                ViewBag.Songs = new List<Top2000Song>();
                ViewBag.TotalPages = 1;
                ViewBag.CurrentPage = 1;
                ViewBag.SelectedTop2000Year = top2000Year;
                ViewBag.SelectedArtist = artist;
                ViewBag.SortBy = sortBy;
                ViewBag.SearchQuery = searchQuery;
                ViewBag.SearchType = searchType;
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
                ViewBag.SelectedTop2000Year = top2000Year;
                ViewBag.SelectedArtist = artist;
                ViewBag.SortBy = sortBy;
                ViewBag.SearchQuery = searchQuery;
                ViewBag.SearchType = searchType;
                return View();
            }

            List<Top2000Song> songs = apiResponse.songs.ToObject<List<Top2000Song>>();

            ViewBag.Songs = songs;
            ViewBag.TotalPages = (int)apiResponse.totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.SelectedTop2000Year = top2000Year;
            ViewBag.SelectedArtist = artist;
            ViewBag.SortBy = sortBy;
            ViewBag.SearchQuery = searchQuery;
            ViewBag.SearchType = searchType;

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
                return View("~/Views/ArtiestInfo/Index.cshtml");
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

            return View("~/Views/ArtiestInfo/Index.cshtml");
        }

        public async Task<IActionResult> SongDetail(int songId)
        {
            var apiUrl = $"https://localhost:7020/api/songs/{songId}";
            var response = await _httpClient.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("API Error: " + response.StatusCode);
                return RedirectToAction("Index");
            }

            var json = await response.Content.ReadAsStringAsync();
            var song = JsonConvert.DeserializeObject<Top2000Song>(json);

            if (song == null)
            {
                Console.WriteLine("API response is null.");
                return RedirectToAction("Index");
            }

            ViewBag.Song = song;
            ViewBag.SongId = song.SongId;
            ViewBag.Title = song.Titel;
            ViewBag.ArtistName = song.Artiest?.Naam;
            ViewBag.ArtistWiki = song.Artiest?.Wiki;
            ViewBag.Year = song.Jaar;
            ViewBag.DurationMs = song.DurationMs;
            ViewBag.Afbeelding = song.Afbeelding;
            ViewBag.Youtube = song.Youtube;
            ViewBag.Popularity = song.Popularity;
            ViewBag.SpotifyUrl = song.SpotifyUrls;

            return View("~/Views/SongInfo/Index.cshtml");
        }
    }
}