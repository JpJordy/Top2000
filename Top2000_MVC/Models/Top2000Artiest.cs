using System.ComponentModel.DataAnnotations;

namespace Top2000_MVC.Models
{
    public class Top2000Artist
    {
        public int ArtiestId { get; set; }
        public string Naam { get; set; } = string.Empty;
        public string? Wiki { get; set; }
        public List<string>? Genres { get; set; }
        public string? Foto { get; set; }
    }
}
