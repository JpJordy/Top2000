using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Top2000_MVC.Models
{
    public class Top2000Song
    {
        public int SongId { get; set; }

        [Required]
        public string Titel { get; set; } = string.Empty;

        public int Jaar { get; set; }

        public string? Afbeelding { get; set; }
        public string? Lyrics { get; set; }
        public string? Youtube { get; set; }

        public int ArtiestId { get; set; }
        public Top2000Artist? Artiest { get; set; }

        public int DurationMs { get; set; }
        public int? Popularity { get; set; }
        public string? SpotifyUrls { get; set; }

        public List<Top2000Notering> Noteringen { get; set; } = new List<Top2000Notering>();
        public int? VorigePositie { get; set; }
        public int? Verschil { get; set; }
    }

    public class Top2000Notering
    {
        public int Jaar { get; set; } 
        public int Positie { get; set; }
    }
}
