using Microsoft.AspNetCore.Mvc;

namespace Top2000_API.Models
{
    public class Lijst
    {
        public int LijstId { get; set; }
        public int SongId { get; set; }
        public int Top2000JaarId { get; set; }
        public int Positie { get; set; }

        public Song Song { get; set; } = null!;
        public Top2000Jaar Top2000Jaar { get; set; } = null!;
    }

}
