using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Top2000_API.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using Top2000_MVC.Models;
using System.Linq;
using static Top2000_API.Controllers.SongsController;

[Route("api/artists")]
[ApiController]
public class ArtistsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly HttpClient _httpClient;

    public ArtistsController(ApplicationDbContext context)
    {
        _context = context;
        _httpClient = new HttpClient();
    }

    // Methode om artiesteninfo van Spotify op te halen (waaronder foto)
    private async Task<ArtistDTO> GetArtistInfoAsync(string artistName)
    {
        var clientId = "586a81230c214d4680827fa11c07357a"; // Spotify Client ID
        var clientSecret = "8ea52916b7dc4ec8985fab26c97ca725"; // Spotify Client Secret

        var authUrl = "https://accounts.spotify.com/api/token";
        var authRequestBody = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("grant_type", "client_credentials") });

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(clientId + ":" + clientSecret)));

        var authResponse = await _httpClient.PostAsync(authUrl, authRequestBody);
        var authResponseJson = JsonConvert.DeserializeObject<dynamic>(await authResponse.Content.ReadAsStringAsync());
        var accessToken = authResponseJson.access_token;

        var searchUrl = $"https://api.spotify.com/v1/search?q={Uri.EscapeDataString(artistName)}&type=artist&limit=1";
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

        var searchResponse = await _httpClient.GetStringAsync(searchUrl);
        var searchResult = JsonConvert.DeserializeObject<dynamic>(searchResponse);

        if (searchResult.artists.items.Count == 0)
        {
            return null; // Als de artiest niet gevonden wordt
        }

        var artist = searchResult.artists.items[0];
        var artistImage = artist.images.Count > 0 ? (string)artist.images[0].url : null;

        return new ArtistDTO
        {
            Naam = artist.name,
            Foto = artistImage,
            Genres = artist.genres.ToObject<List<string>>() ?? new List<string>()
        };
    }

    // Endpoint om alle artiesten op te halen
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<Top2000Artist>>> GetAllArtists(int page = 1, int pageSize = 9)
    {
        var artists = await _context.Artiesten
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var artistDtos = new List<ArtistDTO>();

        foreach (var artist in artists)
        {
            var artistInfo = await GetArtistInfoAsync(artist.Naam);
            if (artistInfo != null)
            {
                artistDtos.Add(artistInfo);
            }
        }

        var response = new
        {
            Artists = artistDtos,
            TotalPages = (int)Math.Ceiling((double)_context.Artiesten.Count() / pageSize)
        };

        return Ok(response);
    }
}
