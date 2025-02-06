using Microsoft.AspNetCore.Mvc;

namespace Top2000_API.Models
{
    public class Lijst
    {
        public int LijstId { get; set; }
        public int SongId { get; set; }
        public int Jaar { get; set; }
        public int Positie { get; set; }

        public Song Song { get; set; } = null!;
    }

}
