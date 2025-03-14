using Microsoft.AspNetCore.Mvc;

namespace Top2000_API.Models
{
    public class Song
    {
        public int SongId { get; set; }
        public int ArtiestId { get; set; }
        public required string Titel { get; set; }
        public int Jaar { get; set; }
        public string? Afbeelding { get; set; }
        public string? Lyrics { get; set; }
        public string? Youtube { get; set; }

        public Artiest Artiest { get; set; } = null!;
        public ICollection<Lijst> Lijsten { get; set; } = new List<Lijst>();
    }

    public class Lijsten
    {
        public int LijstId { get; set; }
        public int SongId { get; set; }
        public int Jaar { get; set; }
        public int Positie { get; set; }

        public Song? Song { get; set; }
    }
}
