using Microsoft.EntityFrameworkCore;

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
    }
}
