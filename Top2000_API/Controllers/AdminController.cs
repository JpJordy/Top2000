using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Top2000_API.Data;
using System.Threading.Tasks;

namespace Top2000_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpDelete("deleteUser/{username}")]
        public async Task<IActionResult> DeleteUser(string username)
        {
            // Zoek de gebruiker in de database
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);

            if (user == null)
            {
                return NotFound("Gebruiker niet gevonden.");
            }

            // Verwijder de gebruiker
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Gebruiker succesvol verwijderd." });
        }
    }
}
