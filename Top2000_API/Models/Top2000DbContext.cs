using Microsoft.EntityFrameworkCore;
using Top2000_MVC.Models;

namespace Top2000_API.Models
{
    public class Top2000DbContext : DbContext
    {
        public Top2000DbContext(DbContextOptions<Top2000DbContext> options)
            : base(options)
        {
        }

        public DbSet<Song> Songs { get; set; }
        public DbSet<Artiest> Artiesten { get; set; }
        public DbSet<Lijst> Lijsten { get; set; } = null!;
    }
}
