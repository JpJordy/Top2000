using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Top2000_API.Models;

namespace Top2000_API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        // Add this constructor to accept DbContextOptions
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)  // Pass options to the base constructor
        { }

        public DbSet<Artiest> Artiesten { get; set; } = null!;
        public DbSet<Song> Songs { get; set; } = null!;
        public DbSet<Lijst> Lijsten { get; set; } = null!;
        public DbSet<ApplicationUser> Users { get; set; } = null!;
    }

    // ApplicationUser class for Identity
   
}
