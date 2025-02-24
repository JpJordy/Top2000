using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Top2000_API.Data
{

    public class ApplicationUser : IdentityUser
    {

        [Key]
        public int Id { get; set; }



        [Required]
        public string PasswordHash { get; set; } // Wachtwoord wordt gehasht opgeslagen
    }
}
