using System.ComponentModel.DataAnnotations;

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
        public int DurationMs { get; set; }  // Voeg deze regel toe
    }

    public class Top2000Artist
    {
        public int ArtiestId { get; set; }
        public string Naam { get; set; } = string.Empty;
        public string? Wiki { get; set; }
        public string? Biografie { get; set; }
        public string? Foto { get; set; }
    }
}
