using Microsoft.AspNetCore.Mvc;

namespace Top2000_API.Models
{
    public class Top2000Jaar
    {
        public int Top2000JaarId { get; set; }
        public int Jaar { get; set; }

        public ICollection<Lijst> Lijsten { get; set; } = new List<Lijst>();
    }
}
