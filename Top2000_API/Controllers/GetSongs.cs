using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Top2000_API.Models;

namespace Top2000_API.Controllers
{
    [ApiController]
    [Route("api/songs")]
    public class SongsController : ControllerBase
    {
        private readonly Top2000DbContext _context;
        private readonly HttpClient _httpClient;

        public SongsController(Top2000DbContext context)
        {
            _context = context;
            _httpClient = new HttpClient();
        }

        public class SongWithArtistDTO
        {
            public int SongId { get; set; }
            public string? Titel { get; set; }
            public int Jaar { get; set; }
            public string? Afbeelding { get; set; }
            public string? Lyrics { get; set; }
            public string? Youtube { get; set; }
            public ArtistDTO? Artiest { get; set; }
            public int DurationMs { get; set; }  
        }


        public class ArtistDTO
        {
            public int ArtiestId { get; set; }
            public string? Naam { get; set; }
            public string? Wiki { get; set; }
            public string? Biografie { get; set; }
            public string? Foto { get; set; }
        }

        private async Task<(string, int)> GetTrackInfoAsync(string songName, string artistName = null)
        {
            var clientId = "586a81230c214d4680827fa11c07357a";
            var clientSecret = "8ea52916b7dc4ec8985fab26c97ca725";

            var authUrl = "https://accounts.spotify.com/api/token";
            var authRequestBody = new FormUrlEncodedContent(new[]
            {
        new KeyValuePair<string, string>("grant_type", "client_credentials")
    });

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(clientId + ":" + clientSecret)));

            var authResponse = await _httpClient.PostAsync(authUrl, authRequestBody);
            var authResponseContent = await authResponse.Content.ReadAsStringAsync();
            var authResponseJson = JsonConvert.DeserializeObject<dynamic>(authResponseContent);

            var accessToken = authResponseJson.access_token;

            var query = $"track:{songName}";
            if (!string.IsNullOrEmpty(artistName))
            {
                query += $" artist:{artistName}";
            }

            var searchUrl = $"https://api.spotify.com/v1/search?q={Uri.EscapeDataString(query)}&type=track&limit=1";
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

            var searchResponse = await _httpClient.GetStringAsync(searchUrl);
            var searchResult = JsonConvert.DeserializeObject<dynamic>(searchResponse);

            if (searchResult.tracks.items.Count > 0)
            {
                var albumCoverUrl = searchResult.tracks.items[0].album.images[0].url;
                var durationMs = searchResult.tracks.items[0].duration_ms;  
                return (albumCoverUrl, durationMs);  
            }

            return ("No album cover found.", 0); 
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SongWithArtistDTO>>> GetSongs(int page = 1, int pageSize = 7)
        {
            var skip = (page - 1) * pageSize;

            var songs = await _context.Songs
                .Include(s => s.Artiest)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            var songDtos = new List<SongWithArtistDTO>();

            foreach (var s in songs)
            {
                var (albumCoverUrl, durationMs) = await GetTrackInfoAsync(s.Titel, s.Artiest.Naam);

                var songDto = new SongWithArtistDTO
                {
                    SongId = s.SongId,
                    Titel = s.Titel,
                    Jaar = s.Jaar,
                    Afbeelding = albumCoverUrl,
                    Lyrics = s.Lyrics,
                    Youtube = s.Youtube,
                    DurationMs = durationMs,
                    Artiest = new ArtistDTO
                    {
                        ArtiestId = s.Artiest.ArtiestId,
                        Naam = s.Artiest.Naam,
                        Wiki = s.Artiest.Wiki,
                        Biografie = s.Artiest.Biografie,
                        Foto = s.Artiest.Foto
                    }
                };

                songDtos.Add(songDto);
            }

            var totalSongs = await _context.Songs.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalSongs / pageSize);

            var response = new
            {
                Songs = songDtos,
                TotalPages = totalPages
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SongWithArtistDTO>> GetSong(int id)
        {
            var song = await _context.Songs
                .Include(s => s.Artiest)
                .FirstOrDefaultAsync(s => s.SongId == id);

            if (song == null) return NotFound();

            var (albumCoverUrl, durationMs) = await GetTrackInfoAsync(song.Titel, song.Artiest.Naam);

            var durationMinutes = durationMs / 60000;
            var durationSeconds = (durationMs % 60000) / 1000; 

            var songDto = new SongWithArtistDTO
            {
                SongId = song.SongId,
                Titel = song.Titel,
                Jaar = song.Jaar,
                Afbeelding = albumCoverUrl, 
                Lyrics = song.Lyrics,
                Youtube = song.Youtube,
                Artiest = new ArtistDTO
                {
                    ArtiestId = song.Artiest.ArtiestId,
                    Naam = song.Artiest.Naam,
                    Wiki = song.Artiest.Wiki,
                    Biografie = song.Artiest.Biografie,
                    Foto = song.Artiest.Foto
                }
            };

            return Ok(songDto);
        }

        [HttpGet("title/{title}")]
        public async Task<ActionResult<IEnumerable<SongWithArtistDTO>>> GetSongsByTitle(string title)
        {
            var songs = await _context.Songs
                .Include(s => s.Artiest)
                .Where(s => s.Titel.Contains(title))
                .OrderBy(s => s.Titel.StartsWith(title) ? 0 : 1)
                .ThenBy(s => s.Titel)
                .ToListAsync();

            var songDtos = new List<SongWithArtistDTO>();

            foreach (var song in songs)
            {
                var (albumCoverUrl, durationMs) = await GetTrackInfoAsync(song.Titel, song.Artiest.Naam);

                var durationMinutes = durationMs / 60000;  
                var durationSeconds = (durationMs % 60000) / 1000;

                var songDto = new SongWithArtistDTO
                {
                    SongId = song.SongId,
                    Titel = song.Titel,
                    Jaar = song.Jaar,
                    Afbeelding = albumCoverUrl, 
                    Lyrics = song.Lyrics,
                    Youtube = song.Youtube,
                    Artiest = new ArtistDTO
                    {
                        ArtiestId = song.Artiest.ArtiestId,
                        Naam = song.Artiest.Naam,
                        Wiki = song.Artiest.Wiki,
                        Biografie = song.Artiest.Biografie,
                        Foto = song.Artiest.Foto
                    }
                };

                songDtos.Add(songDto);
            }

            return Ok(songDtos);
        }

        [HttpPost]
        public async Task<ActionResult<Song>> CreateSong(Song song)
        {
            _context.Songs.Add(song);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetSong), new { id = song.SongId }, song);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSong(int id, Song updatedSong)
        {
            if (id != updatedSong.SongId) return BadRequest();

            _context.Entry(updatedSong).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
