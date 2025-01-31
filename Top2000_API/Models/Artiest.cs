using Microsoft.AspNetCore.Mvc;

namespace Top2000_API.Models
{
    public class Artiest
    {
        public int ArtiestId { get; set; }
        public required string Naam { get; set; }
        public string? Wiki { get; set; }
        public string? Biografie { get; set; }
        public string? Foto { get; set; }

        public ICollection<Song> Songs { get; set; } = new List<Song>();
    }

}
