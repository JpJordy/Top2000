using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Top2000_API.Data;
using System.Threading.Tasks;
using Top2000_API.Models;

namespace Top2000_API.Controllers
{
    [ApiController]
    [Route("api/songs")]
    public class SongsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;

        public SongsController(ApplicationDbContext context)
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
            public int? Popularity { get; set; }
            public string? SpotifyUrls { get; set; }
        }


        public class ArtistDTO
        {
            public int ArtiestId { get; set; }
            public string? Naam { get; set; }
            public string? Wiki { get; set; }
            public string? Foto { get; set; }
            public List<string>? Genres { get; set; }
        }


        private async Task<(string albumCoverUrl, int durationMs, int popularity, string spotifyUrl)> GetTrackInfoAsync(string songName, string artistName = null)
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
                var track = searchResult.tracks.items[0];
                var albumCoverUrl = track.album.images[0].url;
                var durationMs = track.duration_ms;
                var popularity = track.popularity; 
                var spotifyUrl = track.external_urls.spotify; 

                return (albumCoverUrl, durationMs, popularity, spotifyUrl);
            }

            return ("No album cover found.", 0, 0, "No Spotify URL found.");
        }

        private async Task<ArtistDTO> GetArtistInfoAsync(string artistName)
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

            var searchUrl = $"https://api.spotify.com/v1/search?q={Uri.EscapeDataString(artistName)}&type=artist&limit=1";
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

            var searchResponse = await _httpClient.GetStringAsync(searchUrl);
            var searchResult = JsonConvert.DeserializeObject<dynamic>(searchResponse);

            if (searchResult.artists.items.Count == 0)
            {
                return null; 
            }

            var artist = searchResult.artists.items[0];
            var artistId = artist.id; 
            var artistImage = artist.images.Count > 0 ? (string)artist.images[0].url : null;
            var genres = artist.genres.ToObject<List<string>>() ?? new List<string>();

            return new ArtistDTO
            {
                Naam = artist.name,
                Foto = artistImage,
                Genres = genres,
                Wiki = artist.wiki
            };
        }


        [HttpGet("artist/{artistName}")]
        public async Task<ActionResult<ArtistDTO>> GetArtist(string artistName)
        {
            var artistInfo = await GetArtistInfoAsync(artistName);
            if (artistInfo == null) return NotFound("Artiest niet gevonden");

            return Ok(artistInfo);
        }

        [HttpGet("artist/{artistName}/songs-per-year")]
        public async Task<ActionResult<Dictionary<int, int>>> GetSongsPerYear(string artistName)
        {
            var artist = await _context.Artiesten.FirstOrDefaultAsync(a => a.Naam == artistName);
            if (artist == null)
            {
                return NotFound("Artiest niet gevonden");
            }

            var songsPerYear = await _context.Lijsten
                .Where(l => _context.Songs.Any(s => s.SongId == l.SongId && s.ArtiestId == artist.ArtiestId))
                .GroupBy(l => l.Jaar)
                .Select(g => new { Jaar = g.Key, Aantal = g.Count() })
                .ToDictionaryAsync(g => g.Jaar, g => g.Aantal);

            return Ok(songsPerYear);
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<SongWithArtistDTO>>> GetSongs(int page = 1, int pageSize = 7, string sortBy = "positie", int? top2000Year = null)
        {
            var skip = (page - 1) * pageSize;

            IQueryable<Song> songsQuery = _context.Songs.Include(s => s.Artiest);

            if (top2000Year.HasValue)
            {
                var songIdsInTop2000Year = _context.Lijsten
                    .Where(l => l.Jaar == top2000Year.Value)
                    .Select(l => l.SongId)
                    .ToList();

                songsQuery = songsQuery.Where(s => songIdsInTop2000Year.Contains(s.SongId));
            }

            switch (sortBy.ToLower())
            {
                case "artiest":
                    songsQuery = songsQuery.OrderBy(s => s.Artiest.Naam);
                    break;

                case "jaar":
                    songsQuery = songsQuery.OrderBy(s => s.Jaar);
                    break;

                case "titel":
                    songsQuery = songsQuery.OrderBy(s => s.Titel);
                    break;

                case "positie":
                default:
                    songsQuery = from song in songsQuery
                                 join lijst in _context.Lijsten on song.SongId equals lijst.SongId
                                 where top2000Year == null || lijst.Jaar == top2000Year
                                 orderby lijst.Positie
                                 select song;
                    break;
            }


            var songs = await songsQuery.Skip(skip).Take(pageSize).ToListAsync();

            var songDtos = new List<SongWithArtistDTO>();

            foreach (var s in songs)
            {
                var (albumCoverUrl, durationMs, popularity, spotifyUrl) = await GetTrackInfoAsync(s.Titel, s.Artiest.Naam);

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

            var (albumCoverUrl, durationMs, popularity, spotifyUrls) = await GetTrackInfoAsync(song.Titel, song.Artiest.Naam);

            var songDto = new SongWithArtistDTO
            {
                SongId = song.SongId,
                Titel = song.Titel,
                Jaar = song.Jaar,
                Afbeelding = albumCoverUrl, 
                Lyrics = song.Lyrics,
                Youtube = song.Youtube,
                DurationMs = durationMs,
                Popularity = popularity,
                SpotifyUrls = spotifyUrls,
                Artiest = new ArtistDTO
                {
                    ArtiestId = song.Artiest.ArtiestId,
                    Naam = song.Artiest.Naam,
                    Wiki = song.Artiest.Wiki,
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
                var (albumCoverUrl, durationMs, popularity, spotifyUrls) = await GetTrackInfoAsync(song.Titel, song.Artiest.Naam);

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
