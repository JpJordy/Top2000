﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Top2000_API.Data;
using System.Threading.Tasks;
using Top2000_API.Models;

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
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);

            if (user == null)
            {
                return NotFound("Gebruiker niet gevonden.");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Gebruiker succesvol verwijderd." });
        }

        [HttpGet("getUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var usersWithRoles = await _context.Users
                .Select(u => new
                {
                    u.UserName,
                    u.Role
                })
                .ToListAsync();

            return Ok(usersWithRoles);
        }

        [HttpPost("updateRole/{username}/{role}")]
        public async Task<IActionResult> UpdateRole(string username, string role)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);

            if (user == null)
            {
                return NotFound("Gebruiker niet gevonden.");
            }

            user.Role = role;

            await _context.SaveChangesAsync();

            return Ok(new { message = $"Rol van gebruiker {username} succesvol gewijzigd naar {role}." });
        }

        [HttpPost("removeAdmin/{username}")]
        public async Task<IActionResult> RemoveAdmin(string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);

            if (user == null)
            {
                return NotFound("Gebruiker niet gevonden.");
            }

            user.Role = "User";

            await _context.SaveChangesAsync();

            return Ok(new { message = $"Rol van gebruiker {username} succesvol gewijzigd naar User." });
        }

        [HttpGet("getSong/{title}")]
        public async Task<IActionResult> GetSong(string title)
        {
            var song = await _context.Songs.FirstOrDefaultAsync(s => s.Titel == title);
            if (song == null)
            {
                return NotFound("Nummer niet gevonden.");
            }

            return Ok(song);
        }

        [HttpPut("updateSong/{title}")]
        public async Task<IActionResult> UpdateSong(string title, [FromBody] UpdateSongDto songDto)
        {
            var song = await _context.Songs.FirstOrDefaultAsync(s => s.Titel == title);
            if (song == null)
            {
                return NotFound("Nummer niet gevonden.");
            }

            song.Lyrics = songDto.Lyrics;
            song.Afbeelding = songDto.Afbeelding;
            song.Youtube = songDto.Youtube;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Nummergegevens succesvol bijgewerkt." });
        }

        [HttpGet("getArtiest/{naam}")]
        public async Task<IActionResult> GetArtiest(string naam)
        {
            var artiest = await _context.Artiesten.FirstOrDefaultAsync(a => a.Naam == naam);
            if (artiest == null)
            {
                return NotFound("Artiest niet gevonden.");
            }

            return Ok(artiest);
        }


        [HttpPut("updateArtiest/{naam}")]
        public async Task<IActionResult> UpdateArtiest(string naam, [FromBody] UpdateArtistDto artiestDto)
        {
            var artiest = await _context.Artiesten.FirstOrDefaultAsync(a => a.Naam == naam);
            if (artiest == null)
            {
                return NotFound("Artiest niet gevonden.");
            }

            artiest.Wiki = artiestDto.Wiki;
            artiest.Foto = artiestDto.Foto;
            artiest.Biografie = artiestDto.Biografie;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Artiestgegevens succesvol bijgewerkt." });
        }
    }
}
