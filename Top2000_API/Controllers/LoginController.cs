using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Top2000_API.Data;
using Top2000_API.Models;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Top2000_MVC.Models;

namespace Top2000_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = _context.Users.FirstOrDefault(u => u.UserName == model.Username); 
            if (user == null || user.PasswordHash != HashPassword(model.Password))
                return Unauthorized("Ongeldige inloggegevens.");

            // Hier zou je normaal een JWT token genereren, maar voor nu sturen we een simpele response 
            return Ok(new { message = "Login succesvol!", username = user.UserName });
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
    }
}
